using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Action的封装
/// </summary>
public class GameAction {
    private List<Action> source = new List<Action>();

    public void Add(Action action) {
        source.Add(action);
    }

    public static GameAction operator +(GameAction source, Action add) {
        source.Add(add);
        return source;
    }

    public void Invoke() {
        foreach (var action in source) {
            action.Invoke();
        }
    }
}