# Game Time

Dynamic time tracking for Unity

## Installation
This project uses [Odin Inspector](https://assetstore.unity.com/packages/tools/utilities/odin-inspector-and-serializer-89041), which I cannot redistribute. If you don't own Odin Inspector, I would highly recommend purchasing it otherwise you won't be able to serialize interface instances as members which completely breaks this solution.

#### Package Manager
##### Git Extension
First, do yourself a favor and add the [UPM Git Extension](https://github.com/mob-sakai/UpmGitExtension) package to your project. This package makes git packages many times easier to use in your project. simply add `https://github.com/mob-sakai/UpmGitExtension.git` as a new package via the git option in the package manager. Afterwords, reopen the Package Manager.

Next, add this repo. In the top left, you will find a git logo. This button will show a small menu for adding git packages to your project. add `https://github.com/FedoraDevStudios/Game-Time.git` in the `Repository URL` box and hit `Find Versions`. Select the latest version and then `Install Package`.

#### UPM Upgrade
If you added the Git Extension package, then you can change the installed version just like any other package.

#### Manual Installation
This can be added as a dependency to your Unity project manually. You just need to add a reference to this repo to your project's `Packages/manifest.json` file. Be sure to switch `[version]` with whichever release you would prefer, e.g. `.git#1.0.0`.

```js
{
	"dependencies": {
		...,
		"com.fedoradev.paneltabber": "https://github.com/FedoraDevStudios/Game-Time.git#[version]"
	}
}
```

#### Manual Upgrade
After installing manually, you have to change both `Packages/manifest.json` and `Packages/packages-lock.json`. In the former, simply update the dependency with the version you wish to pull. In the lock file, you need to remove the entry for the package. This entry is a few lines long and everything needs to be deleted, including the curly braces. After this is successfully completed, moving back to Unity will force the application to download the desired version.

## Usage
> Please note that Odin has technically deprecated support for SerializedMonoBehaviour with prefabs. The prefabs that I'm going to cover below seem to be functioning fine in my testing. If you have issues with the prefabs missing data, then you likely won't be able to set this up the easy way.

### Quick Guide
I've included 4 prefabs in the package which can be accessed via `Packages -> Game Time -> Examples` in the Project panel. Below, I will explain what each one is for, however it is important to understand what's going on. Each Time Piece contains a component called `GameTimeBehaviour`. This component simply passes in deltaTime to the `IGameTime` field's `Tick` method. By default, only 1 `IGameTime` is provided called `DefaultGameTime`. Within it, there is a list of something called `ITimeUnit`. Each part of time would be made into a unit of time, i.e. seconds, minutes, days, and even years. Each unit of time contains a conversion rate that defines how many of this time unit it takes to make 1 of the next unit. For seconds, the conversion rate would be 60 because there are 60 seconds per minute.

There are 2 given implementations of `ITimeUnit`; `DefaultTimeUnit` and `RealisticMonthTimeUnit`. You are completely free to implement your own `ITimeUnit`s and plug them in here. Each unit of time has an event attached that is configurable in Unity so you don't have to create any dependancies to the system. The events are currently configured to fire once when the associated time has changed and sends the current value of that unit to the event's receivers.

You may notice that the Year unit of each time piece is set to -1. This just means to count as high as possible, so no conversion is necessary.

### Prefabs
#### Simple Time Piece
Simple Time Piece is exactly what it says it is. Each unit of time follows real-world time keeping, however the days per month is a static 30 days. Every `ITimeUnit` used here is a `DefaultTimeUnit`.

#### Realistic Time Piece
Realistic Time Piece uses the same Time Units as Simple Time Piece for Seconds, Minutes, and Hours. The last unit of time is the Days, Months, and Years combined into a single `ITimeUnit` that accurately represents the changes in days/month that we see in the real world, including leap years. This Time Unit was made to handle even large jumps in time spanning multiple days without becoming out-of-sync with the dynamic days/month ratio.

#### Decimal Time Piece
Decimal Time Piece is inspired by [Decimal Time](https://en.wikipedia.org/wiki/Decimal_time). The Days, Months, and Years units are using the same Time Unit as the Realistic Time Piece, however the Second, Minute, and Hour units use a base-10 conversion ratio. Additionally, the Time Scale is set to 1.136 by default. This should make 1 decimal day the same length as 1 regular day. With testing, the decimal day is generally ~20 minutes behind over the course of 1 day which is likely due to some floating-point rounding errors that occur when running the clock super fast. I have not tested a full day's time at a 1:1 ratio, however mathematically they should align correctly.

#### Fantasy Time Piece
This time piece is the reason I created this modular system. For the game I'm working on, I know that I will have a weird time system for the world I'm building, however I have yet to figure out the exact details for the system. This example adds a fourth level to the clock after the hour unit. Every 4 hours, we get 1 creatively-named 'fours-hour'. Every 6 fours-hours, we get 1 day. This means we still have 24 hours in a day, but we have 6 distinct sub-sections of that day. Additionally, instead of 12 Months, we have 4 Seasons. This is great for a game that condenses the full year into 4 would-be months.

## Details
### ITimeUnit
`ITimeUnit` simply requires a single method; Tick. This method takes in a float `tickTime` that should be added to the unit's current time, then calculate how many times this unit has lapsed and return that value. I.E. if a minute unit of time received 220 in tickTime, then the current time would end up as 40 with 3 lapses. In a realistic time setup, this would equate to 3 minutes and 40 seconds.

```C#
public interface ITimeUnit
{
	int Tick(float tickTime);
}
```

The example is pretty flexible, allowing 0 and 1 indexed time units. For example, a minute unit would start at 0 whereas a day unit would start at 1. After adding the tickTime to the current value, we check for lapses and reduce the current value back down to one within the conversion rate. Then, we invoke the changed event with that value and finally return the lapse count.

```C#
public class DefaultTimeUnit : ITimeUnit
{
	[SerializeField, FoldoutGroup("$Name")] bool _oneIndexed = false;
	[SerializeField, FoldoutGroup("$Name")] int _conversionRate = 1;
	[SerializeField, FoldoutGroup("$Name")] float _current = 0f;
	[SerializeField, HideLabel, BoxGroup("$Name/Change Event")] UnityEvent<float> _timeChanged = new UnityEvent<float>();

	int ConversionRate => _conversionRate == -1 ? int.MaxValue : _conversionRate;

	public int Tick(float tickTime)
	{
		_current += tickTime;

		int lapses = _oneIndexed ?
			Mathf.FloorToInt((_current - 1) / ConversionRate) :
			Mathf.FloorToInt(_current / ConversionRate);

		_current -= ConversionRate * lapses;

		if (lapses > 0)
			_timeChanged?.Invoke(_current);

		return lapses;
	}
}
```

### IGameTime
`IGameTime` also only requires `Tick` to be defined, only this time we don't worry about returning any information.

```C#
public interface IGameTime
{
	void Tick(float tickTime);
}
```

The default implementation is rather simple. We contain a list of `ITimeUnit`s. When `Tick` is called, we propogate the time through each item in the list until the lapses hit 0 or we run out of time units. We have also added a time scale for quick modification of time speed.

```C#
public class DefaultGameTime : IGameTime
{
	[SerializeField, HideLabel, BoxGroup("Time Scale")] float _timeScale;
	[SerializeField, HideLabel, BoxGroup("Time Units")] List<ITimeUnit> _timeUnit;

	public void Tick(float tickTime)
	{
		tickTime *= _timeScale;

		for (int i = 0; i < _timeUnit.Count; i++)
		{
			tickTime = _timeUnit[i].Tick(tickTime);

			if (tickTime <= 0)
				break;
		}
	}
}
```

In addition to the default implementation, we have also added `GameTimeBehaviour`. This implementation doesn't require much explanation as all it does is hold an `IGameTime` and pass the deltaTime from Update into it.