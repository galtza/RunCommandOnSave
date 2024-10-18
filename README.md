# RunCommandOnSave

Visual Studio Extension (VSIX) that allows running any command as you save files (Pre/Post save).

### Targets
It supports VS2019 and VS2022.

### Configure
A `.runcommandonsave` file is a directory-level configuration file with INI format. All the documents in the folder and subfolders will abide by the configuration. We can override **the whole file** by creating another `.runcommandonsave` file in a subfolder.

It can contain 3 possible sections: `[PreSave]`, `[PostSave]` or `[Debug]` like in this example. The debug section only allows to activate it with the key `On`. The other two, allow specifying the commands we want to execute, the excluded extensions (none by default) and the excluded paths (none by default too).

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

You can set specific commands for certain file types using sections like `[PreSave.ext1|ext2|ext3]`, where `ext1`, `ext2`, and `ext3` are the file extensions. This feature allows you to execute relevant commands for designated file types, enabling tailored actions depending on the file being saved.

For example:

```config
[PreSave.js|ts]
Commands=RunLinter|AutoFormat

[PreSave.md]
Commands=RunSpellCheck
```

This configuration ensures that JavaScript and TypeScript files trigger linting and formatting, while Markdown files undergo spell checking upon saving.

> Note that targeted commands specified for certain file extensions replace the general commands in the `[PreSave]` or `[PostSave]` sections. Additionally, any exclusions of extensions and paths specified are applied consistently across all corresponding targeted sections, ensuring uniform handling for file types.
