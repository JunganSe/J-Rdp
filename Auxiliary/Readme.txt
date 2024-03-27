RdpRunner is a tool to automate the editing and launching of rdp files.

It will watch one or more folders for new files.
When a new file is found, several actions can be taken:
- Move the file to another location.
- Edit settings in the file.
- Launch the file.
- Delete the file.

Behavior is controlled with  a json file 'config.json' that must be present in the same directory as the executable.
An example file with two config profiles is provided.

Explanation of config properties:
- name: For user convenience and logging, does not affect the program.
- watchFolder: Path to the directory to watch for new files.
- filter: File name to watch for. Supports wildcards.
- moveToFolder: If provided, the file will be moved to this location before further action is taken.
- launch: Whether to launch the file automatically. Can be true or false.
- delete: Whether to delete the file after all other actions have been completed. Can be true or false.
- settings: An array of strings, which will be appended on separate lines at the end of the file.

Folder paths can be absolute or relative (to the executable).
Path patterns:
- "C:/Foo/Bar"    absolute
- "Foo/Bar"       relative to .exe
- "./Foo/Bar"     relative to .exe
- "../Foo/Bar"    relative to .exe, up one level, then Foo/Bar
- "/Foo/Bar"      relative to root of .exe

Filename wildcards:
- '*'    0 or more characters.
- '?'    exactly 1 character.

Command line arguments on startup:
- PollingInterval: Time in milliseconds between each check for new files. Default: 1000
- HideConsole: Whether to hide the console window. Default: false.

Use the provided bat files to conveniently start and stop the application without the console window, 
and to launch the application on startup. (Create a shortcut in your startup folder, or use the task scheduler.)

Planned for future versions:
- Edit existing settings if present, instead of adding potential duplicates at the end.
- Ability to select log level and turn off logging entirely.
- Ability to change the delay before the file is deleted. (Currently 3 seconds.)