using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Invoker
{
    public bool ExecuteCommands()
    {
        foreach (var command in _commands)
        {
            if(command.Execute() is false)
                return false;
        }
        
        return true;
    }

    public void InDependentExecuteCommands()
    {
        foreach (var command in _commands)
        {
            command.Execute();
        }
    }

    public bool ExecuteCommand(ICommand command)
    {
        return command.Execute();
    }
    
    public void AddCommand(ICommand command)
    {
        _commands.Add(command);
    }
    
    private List<ICommand> _commands = new();
}
