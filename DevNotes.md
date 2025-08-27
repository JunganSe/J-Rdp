# Program control flow

## App initialization

- App launch:
   1. Register cleanup actions for when the app closes.
   2. Initialize logger, reading config from file if present.
   3. Check if program is already running to prevent duplicate process.
   4. Setup GUI. (See below)
   5. Initialize and run the core functionality. (See below)

* Setup GUI
  1. Define and set actions (callbacks) for menu items.
  2. Create a tray icon.
  3. Construct the tray menu, using the callbacks defined above.
  4. (Profile list in menu and menu item checked states will be set after core reads the config file. See below.)

- Initialize and run core functionality:
  1. TODO



## Config file is updated
TODO



## User clicks a menu item
TODO