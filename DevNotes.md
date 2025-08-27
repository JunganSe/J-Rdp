# Program control flow

## App initialization

- App launch:
   1. Register cleanup actions for when the app closes.
   2. Initialize logger, reading config from file if present.
   3. Check if program is already running to prevent duplicate process.
   4. Setup GUI. (See below)
   5. Set callbacks for Core controller, for when the config file is updated and should be updated.
   6. Start running the Core functionality. (See below)

* Setup GUI
  1. Set actions (callbacks) for menu items.
  2. Create a tray icon.
  3. Construct the tray menu, using the callbacks defined above.
  4. (Profile list in menu and menu item checked states will be set after Core reads the config file. See below.)

- Core functionality:
  1. Initialization.
     1. Start watching the config file for changes.
     2. Create a config file if one does not exist.
     3. Read the config file into memory.
     4. Invoke the callback for when the config file is updated. (See below)
     5. Apply settings from config.
     6. Apply enabled, valid profiles from config.
  2. Start main loop. (Runs until app is stopped.)
     1. TODO




## Config file is updated
TODO



## User clicks a menu item
TODO