# Program control flow

## App initialization

- App launch:
   1. Register cleanup actions for when the app closes.
   2. Initialize logger, reading nlog config from file if present.
   3. Check if program is already running to prevent duplicate process.
   4. Setup GUI. (See "Setup GUI" below)
   5. Set callbacks for Core controller, for when the config file is updated and should be updated.
   6. Start running the Core functionality. (See "Core functionality" below)

* Setup GUI:
  1. Set actions (callbacks) for menu items.
  2. Create a tray icon.
  3. Construct the tray menu, using the callbacks defined above.
  4. (Profile list in menu and menu item checked states will be set after Core reads the config file. See "When config file is updated" below.)

- Core functionality:
  1. Initialization.
     1. Start watching the config file for changes.
     2. Create a config file if one does not exist.
     3. Initialize config in memory. (See "Config file is updated" below)
  2. Start main loop. (Runs until app is stopped.)
     1. Check which files exist in watch directory for each profile.
     2. Process each file that didn't exist on the previous check. (See "File processing" below)
     3. Wait according to the polling interval.

* File processing:
  1. Check if file name matches profile filter. Aborts if not.
  2. Move file if path is set in profile.
  3. Apply settings if defined in profile.
  4. Launch the file if set in profile.
  5. Delete the file after a delay, if set in profile.

## Config file is updated

- When ConfigWatcher detects that the config file is changed:
  1. Initialize config in memory.
     1. Read the config file into memory.
     2. Invoke the callback for when the config file is updated. (See "When config file is updated" below)
     3. Apply Core settings from config.
     4. Apply enabled, valid profiles from config.

* When config file is updated:
   1. Core sends a ConfigInfo to the GUI via callback function.
   2. GUI shows or hides the log console according to config.
   3. GUI enables or disables file logging according to config.
   4. GUI menu is updated according to config.
      1. Update menu items checked state.
      2. Update profiles in menu according to config, or show a dummy profile if there are none.


## User clicks a menu item
- When "Show log console" is clicked:
  1. Event "OnClick_ToggleConsole" is triggered.
  1. ConsoleManager is told to show or hide the log console.
  3. CoreManager tells Core controller to update the config.
  4. Core controller tells ConfigManager to update the config file.
  5. (Menu state is updated when the config file is updated. See "When config file is updated" above.)

* When "Log to file" is clicked:
  1. Event "OnClick_ToggleLogToFile" is triggered.
  2. LogManager is told to enable or disable file logging.
  3. CoreManager tells Core controller to update the config.
  4. Core controller tells ConfigManager to update the config file.
  5. (Menu state is updated when the config file is updated. See "When config file is updated" above.)

- When "Open logs folder" is clicked:
  1. Event "OnClick_OpenLogsFolder" is triggered.
  1. CoreManager tells Core controller to open the folder where log files are stored.
  3. Core controller tells LogManager to open the logs folder(s).

* When "Open config file" is clicked:
  1. Event "OnClick_OpenConfigFile" is triggered.
  2. CoreManager tells Core controller to open the config file.
  3. Core controller tells ConfigManager to open the config file in shell. (OS default program.) If no config file exists, one will first be created.

- When a profile is clicked:
  1. Event "OnClick_Profile" is triggered. ProfileInfos are collected with enabled states depending on which profile is clicked and whether the ctrl button is held.
  2. The event invokes a ProfileHandler callback, which triggers CoreManager to tell Core controller to update the config.
  3. Core controller tells ConfigManager to update the config file.
  4. (Menu state is updated when the config file is updated. See "When config file is updated" above.)