Feature: SpecFlowTests
	In order to pass the PREN module
	As PREN student
	I want to make sure that our icarus vehicle is theoretically running as expected

Scenario: The vehicle is on an incline
	Given the vehicle is driving on the parcours
	When the tilt sensor measures an incline
		And the tof sensor returns values which triggers reaction
		And the object detection returns values which triggers reaction
	Then no reaction is triggered

Scenario: The vehicle is on a decline
	Given the vehicle is driving on the parcours
	When the tilt sensor measures a decline
		And the tof sensor returns values which triggers reaction
		And the object detection returns values which triggers reaction
	Then no reaction is triggered

Scenario: The vehicle is on an obstacle or on the ground - tof reaction
	Given the vehicle is driving on the parcours
	When the tilt sensor measures horizonally
		And the tof sensor returns values which triggers reaction
		And the object detection returns values which does not trigger reaction
	Then reaction is triggered

Scenario: The vehicle is on an obstacle or on the ground - object detection reaction
	Given the vehicle is driving on the parcours
	When the tilt sensor measures horizonally
		And the tof sensor returns values which does not trigger reaction
		And the object detection returns values which triggers reaction
	Then reaction is triggered

Scenario: The vehicle is on an obstacle or on the ground - tof and object detection reaction
	Given the vehicle is driving on the parcours
	When the tilt sensor measures horizonally
		And the tof sensor returns values which triggers reaction
		And the object detection returns values which triggers reaction
	Then reaction is triggered

Scenario: The vehicle is on an obstacle or on the ground - no reaction
	Given the vehicle is driving on the parcours
	When the tilt sensor measures horizonally
		And the tof sensor returns values which does not trigger reaction
		And the object detection returns values which does not trigger reaction
	Then no reaction is triggered

Scenario: The vehicle is seeing a horizontal traffic cone
	Given the vehicle is driving on the parcours
	When the tilt sensor measures horizonally
		And the tof sensor returns values which triggers a stop
		And the object detection returns values which triggers a stop
	Then the vehicle stops
	