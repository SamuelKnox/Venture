using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devdog.QuestSystemPro
{
    public interface ITimerHelper : IDisposable
    {
//        void StopAllTimers();
        void StopTimer(int id);
        int StartTimer(float time, Action callbackContinous, Action callbackWhenTimeIsOver);
        int StartTimer(float time, Action callbackWhenTimeIsOver);
    }
}
