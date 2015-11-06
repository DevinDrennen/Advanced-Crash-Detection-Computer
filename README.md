Advanced Crash Detection Computer (ACDC)
========================================

As our senior project my friend and I have decided to create a microcontroller system to detect a bike crash and report the event in various ways.

**Notes:**
There is a snippet of code intended to solve issues with the SDK and VS 2015, however, it should not interfere with other versions of VS.


**Features Planned:**
* Detection
  * Front crash w/o falling over
  * Crash leading fall on side
  * Flips etc
* Reporting
  * Flashing Light
  * Txt message
  * Maybe emmit emergency RF signal

**Hardware:**
* [Netduino 3 WiFi](http://www.netduino.com/netduino3wifi/specs.htm)
* [Razor IMU](https://www.sparkfun.com/products/10736)

![Microcontroller and sensor](/Photos/hardware.png?raw=true)


Here is the way things fit into the case. A phone is shown with the website served by the netduino.

![Microcontroller and sensor](/Photos/collection.png?raw=true)
