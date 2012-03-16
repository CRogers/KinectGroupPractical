using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RobotSimulator.Utility
{
    //For implementing the observer pattern!
    public interface I_Observable<T>
    {
        //Add a listener to the observable object.
        void addListener(T listener);
        //Remove a listener from the observable object.
        void removeListener(T listener);
    }
}
