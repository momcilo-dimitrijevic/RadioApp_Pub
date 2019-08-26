using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace RadioApp.Helpers
{
    public class ObservableExceptionHandler : IObserver<Exception>
    {
        public void OnNext(Exception value)
        {
            if (Debugger.IsAttached) Debugger.Break();

            //Analytics.Current.TrackEvent("MyRxHandler", new Dictionary<string, string>()
            //                                    {
            //                                        {"Type", value.GetType().ToString()},
            //                                        {"Message", value.Message},
            //                                    });

            RxApp.MainThreadScheduler.Schedule(ThreadState.Unknown, (a, b) =>
            {
                throw value;
            });
        }

        public void OnError(Exception error)
        {
            if (Debugger.IsAttached) Debugger.Break();

            //Analytics.Current.TrackEvent("MyRxHandler Error", new Dictionary<string, string>()
            //{
            //    {"Type", error.GetType().ToString()},
            //    {"Message", error.Message},
            //});

            RxApp.MainThreadScheduler.Schedule(ThreadState.Unknown, (a, b) => { throw error; });
        }

        public void OnCompleted()
        {
            if (Debugger.IsAttached) Debugger.Break();

            RxApp.MainThreadScheduler.Schedule(ThreadState.Unknown, (a, b) => { throw new NotImplementedException(); });
        }
    }
}
