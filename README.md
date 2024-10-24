# RunCommandOnSave (v1.1.1)

This Visual Studio Extension (VSIX) lets you run commands when you save files (before and after saving).

### Configure
A `.runcommandonsave` file is a directory-level configuration file with INI format. All the documents in the folder and subfolders will abide by the configuration. We can override **the whole file** by creating another `.runcommandonsave` file in a subfolder.

You can set up rules in a `.runcommandonsave` file in any directory. This file uses the INI format. Any rule you write in this file will apply to all files in that directory and its subdirectories. Putting another `.runcommandonsave` file in a subdirectory will replace **all** the rules from the parent directory.

It can contain 3 possible sections: `[PreSave]`, `[PostSave]` or `[Debug]` like in this example. The debug section only allows to activate it with the key `On`. The other two, allow specifying the commands we want to execute, the excluded extensions (none by default) and the excluded paths (none by default too).

The file can have three sections: `[PreSave]`, `[PostSave]`, or `[Debug]`. Here’s what each section does:

- `[Debug]`: Turn it on with the key `On`.
- `[PreSave]` and `[PostSave]`: These let you specify which commands to run, which file extensions to ignore (none by default), and which paths to exclude (also none by default).

Example configuration:

```config
[Debug]
On = true

; All keys can contain multiple values separated by '|'

[PreSave]
Commands=Edit.FormatDocument|OtherCommand
ExcludeExtensions=.xml|.clang-format|.runcommandonsave
ExcludePaths=third-party|research
```

### Targeted Commands by File Extension

You can set specific commands for different types of files using sections like `[PreSave.ext1|ext2|ext3]`, where `ext1`, `ext2`, and `ext3` are file extensions. This allows you to run appropriate commands based on the type of the file being saved.

For example:

```config
[PreSave.js|ts]
Commands=RunLinter|AutoFormat

[PreSave.md]
Commands=RunSpellCheck
```

This example setup means that _JavaScript_ and _TypeScript_ files will trigger linting and formatting commands, while _Markdown_ files will run a spell check when saved.

> Note: If you specify commands for certain file types, they override the general commands in the `[PreSave]` or `[PostSave]` sections. Also, any excluded extensions and paths you set apply across all targeted sections, ensuring consistent handling for all file types.
