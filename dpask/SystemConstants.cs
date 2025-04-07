namespace dpask
{
    static class SystemConstants
    {
        public static string PersistPathRoot
        {
            get
            {
                var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                return Path.Combine(folder, "BUAADeepseekWebAPI");
            }
        }

        public const string Seperator = "=====================================";

        public const string ShortHelpMessage = "[Usage]: dpask [[options] <question>] | [<feature> [[command] [args]]]";

        public const string LongHelpMessage = """
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
            """;
    }
}
