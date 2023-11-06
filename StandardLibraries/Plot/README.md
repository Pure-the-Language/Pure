# (Experimental) Plot Library

Status: Usable, Expect Changes

Provides an experimental single-entry straightforward easy-to-use and remember 1D/2D plotting functionalities for numerical and catagorical data: `Plot(XAxis, Optional AXisSeries, Type, Settings)`. Alternatively, one could also use explicitly named functions for specific plot types.
This component is only responsible for gathering data and saving as pictures - there is a dedicated window based interactive display for viewing data.
This is NOT part of Pure standard and may be removed without further notice.

Variations:

* `Plot(X, Y, Type, Settings)`
* `Plot(X, Y1-Y20, Type, Settings)`

General configurations:

* `Title`
* `XAxis`
* `YAxis`

Available plot types:

* Signal: affects `SignalSampleRate` option.