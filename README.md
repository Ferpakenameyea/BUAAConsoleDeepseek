# BUAAConsoleDeepseek

一个在本地调用BUAA部署的Deepseek的工具`dpask`，并且提供本地历史管理和调教词管理与注入。



## 框架需求

[.NET 8.0框架](https://dotnet.microsoft.com/zh-cn/download/dotnet/8.0)

## 编译

### Visual studio 自动编译

使用`Visual studio`打开项目 `生成/生成解决方案`



### Dotnet CLI 编译

在终端中切换根目录为`dpask`文件路径下，使用：

````
dotnet build -c Release
````

来完成构建



## 安装与使用

为了能在终端快速使用，推荐将生成的`.exe`可执行文件的根目录加入到系统`PATH`环境变量，这样，您就可以在终端快速使用`dpask`进行调用。



### 凭证管理

截止目前，`dpask`仍然使用朴素的爬虫原理，也就是说，您需要登录`BUAA Deepseek`页面，并复制`Cookie`到目录：

`"C:\Users\<您的系统用户>\AppData\Roaming\BUAADeepseekWebAPI\creds.txt"`



### Hello, world!

在终端中使用：

```
dpask Hello,world!
```

尝试调用Deepseek聊天。



使用

```
dpask help
```

来查看和命令探索您的AI体验



## 命令

完整的命令帮助，您也可以使用`dpask help`来查看这个文本：

```
[Usage]: dpask [[options] <question>] | [<feature> [[command] [args]]]

[Example]:
    dpask "Hello, deepseek!"
=====================================
[options]:
    --no-context | -nc
        Disable context usage, only use the last question and answer.

question:
    your question (or prompt) for deepseek
=====================================
[feature]:
    help | hp
        Show this such a long help message.

    history | h
        List the history of current session.

    clear | c
        Clear the history of current session.

    sessions | s
        Session features

        Subcommands:
            list | l
                List all sessions.
            view | v
                View the session history.
                pattern: 
                    dpask sessions view <session-id>
                    session-id:
                        the id of the session to be viewed
            create | c
                Create a new session.
                pattern:
                    dpask sessions create <name> [comment]
                    name:
                        the name of the session, must be unique
                    comment:
                        the comment of the session, optional
            delete | d
                Delete a session.
            switch | s
                Switch to a session.
            comment | cm
                Comment a session.
                pattern:
                    dpask sessions comment <session-id> <comment>
                    session-id:
                        the id of the session to be commented
                    comment:
                        the comment of the session, natural language

    injections | i
        Inject system prompt

        Subcommands:
            list | l
                List all available injection prompts.
            create | c
                Create a new injection prompt.
                pattern:
                    dpask injections create <prompt> <followed-input> [comment]

                    prompt:
                        the prompt to be injected, natural language
                    followed-input:
                        the input to be followed during injection interaction, natural language
                    comment:
                        the comment of the injection prompt, optional
            delete | d
                Delete an injection prompt.
                pattern:
                    dpask injections delete <id>

                    id:
                        the id of the injection prompt to be deleted
            use | u
                Use an injection prompt to the current session.
                pattern:
                    dpask injections use <id> [session-id]
                    id:
                        the id of the injection prompt to be used
                    session-id:
                        the session to inject prompt, if not specified, use the current session
            comment | cm
                Comment an injection prompt.
                pattern:
                    dpask injections comment <id> <comment>
                    id:
                        the id of the injection prompt to be commented
                    comment:
                        the comment of the injection prompt, natural language
            edit | e
                Edit an injection prompt.
                pattern:
                    dpask injections edit <id> command1 [command2]
                    id:
                        the id of the injection prompt to be edited
                    prompt:
                        the new prompt to be injected, natural language
                    followed-input:
                        the new input to be followed during injection interaction, natural language
                    command:
                        command => <command> <value>
                        each command is a key-value pair, separated by a space

                        -set-prompt <prompt> | -sp <prompt>
                            set the prompt to be injected, natural language

                        -set-followed-input <followed-input> | -sfi <followed-input>
                            set the input to be followed during injection interaction, natural language
=====================================
```

## 免责声明

此软件仅提供学习使用，作者不对提示词注入，询问敏感话题等风险行为以及他们带来的后果承担责任。