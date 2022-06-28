# RunCommandOnSave

Visual Studio Extension (VSIX) that allows running any command as you save files (Pre/Post save).

### Targets
It supports VS2019 and VS2022.

### Configure
A `.runcommandonsave` file is a directory-level configuration file with INI format. All the documents in the folder and subfolders will abide by the configuration. We can override **the whole file** by creating another `.runonsave` file in a subfolder.

It can contain 3 possible sections: `[PreSave]`, `[PostSave]` or `[Debug]` like in this example. The debug section only allows to activate it with the key `On`. The other two, allow specifying the commands we want to execute, the excluded extensions (none by default) and the excluded paths (none by default too).

```config
[Debug]
On = true

; All keys can contain multiple values separated by '|'

[PreSave]
Commands=Edit.FormatDocument|OtherCommand
ExcludeExtensions=.xml|.clang-format|.runcommandonsave
ExcludePaths=third-party|research

[PostSave]
; Etc.
```

If the Debug is on, it will print to the VS output console a message indicating if the saved file was processed.

