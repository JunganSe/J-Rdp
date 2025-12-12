# Program control flow

## App initialization

- App launch:
   1. Register cleanup actions for when the app closes.
   2. Initialize logger, reading nlog config from file if present.
   3. Check if program is already running to prevent duplicate process.
   4. Setup GUI. (See "Setup GUI" below)
   5. Set callbacks for Core controller, and ConsoleManager.
   6. Start running the Core functionality on a separate thread. (See "Core functionality" below)

* Setup GUI:
  1. Set actions (callbacks) for menu items.
  2. Create a tray icon.
  3. Construct the tray menu, using the callbacks defined above.
  4. (Profile list in menu and menu item checked states will be set after Core reads the config file. See "When config file is updated" below.)

- Core functionality:
  1. Initialization.
     1. Start watching the config file for changes. (See "Config file is updated" below)
     2. Create a config file if one does not exist.
     3. Initialize config in memory.
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
   1. Core sends a ConfigInfo object to the GUI via callback function.
   2. GUI shows or hides the log console according to config.
   3. GUI menu is updated according to config.
      1. Update menu items checked state.
      2. Update profiles in menu according to config, or show a dummy profile if there are none.


## User clicks a menu item
* Callbacks for menu item events are initially set in WinApp "Controller.GetTrayCallbacks()".

- When "Show log console" is clicked:
  1. Event "OnClick_ToggleConsole" is triggered.
  2. CoreManager tells Core controller to execute a command.
  3. Core controller tells an ILogDisplayManager to set console visibility
  4. ILogDisplayManager's callback triggers:
     - GUI is updated.
     - Core controller is told to update the config.

* When "Log to file" is clicked:
  1. Event "OnClick_ToggleLogToFile" is triggered.
  2. CoreManager tells Core controller to execute a command.
  3. Core controller tells LogManager to enable or disable file logging.
  4. Core controller tells ConfigManager to update the config file.
  5. (Menu state is updated when the config file is updated. See "When config file is updated" above.)

- When "Open logs folder" is clicked:
  1. Event "OnClick_OpenLogsFolder" is triggered.
  2. CoreManager tells Core controller to execute a command.
  3. Core controller tells LogManager to open the logs folder(s).

* When "Open config file" is clicked:
  1. Event "OnClick_OpenConfigFile" is triggered.
  2. CoreManager tells Core controller to execute a command.
  3. Core controller tells ConfigManager to open the config file in shell. (OS default program.)\
     If no config file exists, one will first be created.

- When a profile is clicked:
  1. Event "OnClick_Profile" is triggered.
  2. ProfileInfo objects are collected with enabled states depending on which profile is clicked and whether the ctrl button is held.
  3. The event invokes a ProfileHandler callback, which triggers CoreManager to tell Core controller to execute a command.
  4. Core controller tells ConfigManager to update the config file.
  5. (Menu state is updated when the config file is updated. See "When config file is updated" above.)



<br/><br/>
# Release procedure

First ensure that the release notes are up to date,\
and that all changes are merged into the `main` branch.

## Automatic release with Github actions

In the Github repository:
1. Run the `Create Release` workflow.
2. Enter a version number. Use a "1.2.3" format without a "v" prefix.
3. A tag is added to the main branch and a release draft is created.\
   Navigate to it in Github and enter the release notes for the version below the template text.
4. Publish the release.

## Manual release

### Create executable files
In Visual studio:
1. When in the `main` branch, right click `WinApp` in solution explorer and select "Publish...".
2. If a publish profile does not exist:
   1. Click "Add a publish profile".
   2. Select "Folder", then click next (twice).
   3. Select export folder location, or leave it at `bin\Release\net8.0-windows\publish\`.\
      Click "Finish", then "Close" when complete.
   4. Click "Show all settings", ensure the following:
      Configuration: Release\
      Target framework: .net8\
      Deployment mode: Framework-dependent\
      Target runtime: win-x86\
      File publish options: "Produce single file" is checked. "ReadyToRun" is unchecked.
3. Click "Publish".\
   When complete, click "Navigate" to open the containing folder.
4. Delete all `.pdb` files.
5. Add the files to a zip named "J-Rdp_0.0.0_win-x86.zip", where "0.0.0" is replaced with the current version number.

### Create a release on GitHub
1. On the GitHub code page, click "Create a new release".
2. Create and select a new tag matching the current version number with a prefix of "v". E.g. "v1.2.3"
3. Select target "main".
4. Set title to "Prerelease v0.0.0", where "0.0.0" is replaced with the current version number.
5. Add the boilerplate part of the description:
   ```
   No installation is needed, unzip and run the exe file.
   See the readme file for more info.

   Requires .NET 8.0 x86 desktop runtime, get it [here](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) ([direct link](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-desktop-8.0.15-windows-x86-installer)).

   ---
   ```
6. After the horizontal rule, add a copy of this version's release notes into the description.
7. Check "Set as a pre-release".
8. Upload the zipped files.
9. Click "Publish release".
