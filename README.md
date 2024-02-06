WIP - Work in progress
---

A tool that watches a folder for files. When a file is found, it is edited and run.\
Primarily intended to auto-insert settings into rdp files before opening the remote connection automatically, as soon as the rdp file is downloaded into a specific folder.

Configurable through a json file:
- Watch folders through absolute or relative path.
- Look for file names using wildcards.
- Upserts settings/lines in the file as configured.
- Optionally move the file to another folder before operations are made.
- Optionally run the file after editing and/or moving.
- Optionally delete the file afterwards.
