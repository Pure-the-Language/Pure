# Plot Window

Status: Experimental

Provides a window display backend for Plot. In current implement, we don't invoke any sockets and rely on temp files so it will not work interactively (can be updated on the spot) - it can still work interactively if we somehow through command line arguments target "single instance" mode).
Notice this component must be implemented as a standalone application, because WPF display requires STA thread and source caller (Pure) may not have a window at all.

Some preliminary standalone use will also be provided.