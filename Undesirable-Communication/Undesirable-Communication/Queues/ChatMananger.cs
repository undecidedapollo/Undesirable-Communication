using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Web;
using Undesirable_Communication.Model.Connection;
using Undesirable_Communication.Model.User;

namespace Undesirable_Communication.Queues
{
    public class ChatManager
    {
        private static ChatManager manager;
        private List<RegisteredUser> PendingUsers {get;set;}

        private List<ChatConnection> CurrentConnections { get; set; }

        private readonly object PendingUsersLock;
        private readonly object CurrentConnectionsLock;

        private ChatManager()
        {
            PendingUsersLock = new object();
            lock (PendingUsersLock)
            {
                CurrentConnectionsLock = new object();
                lock (CurrentConnectionsLock)
                {
                    PendingUsers = new List<RegisteredUser>();
                    CurrentConnections = new List<ChatConnection>();
                }
            }
            
        }

        public static ChatManager GetInstance()
        {
            if(manager == null)
            {
                manager = new ChatManager();
            }

            return manager;
        }

        public void EnqueuePendingUser(RegisteredUser theUser) {
            lock (this.PendingUsersLock)
            {
                this.PendingUsers.Add(theUser);
            }
        }

        public RegisteredUser DequeuePendingUser()
        {
            lock (this.PendingUsersLock)
            {
                var item = this.PendingUsers[0];
                this.PendingUsers.RemoveAt(0);
                return item; 
            }
        }

        private Guid? MakeConnectionIfPossible()
        {
            lock (this.PendingUsersLock)
            {
                if(this.PendingUsers.Count <= 1)
                {
                    return null;
                }

                var user1 = DequeuePendingUser();
                var user2 = DequeuePendingUser();

                var newConnectionGuid = Guid.NewGuid();

                var newConnection = new ChatConnection { User1 = user1, User2 = user2, Id = newConnectionGuid, StartTime = DateTime.Now };

                lock (this.CurrentConnectionsLock)
                {
                    this.CurrentConnections.Add(newConnection);

                    return newConnectionGuid;
                }
            }
        }

        public T QueryPendingUsers<T>(Func<List<RegisteredUser>, T> funcToRun)
        {
            lock (this.PendingUsersLock)
            {
                return funcToRun(this.PendingUsers);
            }
        }

        public void AddKnownConnection(ChatConnection connections)
        {
            lock (this.CurrentConnectionsLock)
            {
                this.CurrentConnections.Add(connections);
            }
        }

        public T QueryKnownConnections<T>(Func<List<ChatConnection>, T> funcToRun)
        {
            lock (this.CurrentConnectionsLock)
            {
                return funcToRun(this.CurrentConnections);
            }
        }

        public void Cleanup()
        {
            var theSpan = TimeSpan.FromSeconds(15);

            QueryKnownConnections((x) =>
            {
                x.RemoveAll(y => y.User1.LastCheckIn < DateTime.Now - theSpan || y.User2.LastCheckIn < DateTime.Now - theSpan);
                return (object)null;
            });

            QueryPendingUsers((x) =>
            {

                x.RemoveAll(y => y.LastCheckIn < DateTime.Now - theSpan);
                return (object)null;
            });
        }

        public static void ConnectionMakerThread_DoWork(object sender, DoWorkEventArgs e)
        {
            var manager = GetInstance();


            while (e.Cancel == false)
            {
                Guid? theGuid = null;

                do
                {
                    theGuid = manager.MakeConnectionIfPossible();
                } while (theGuid != null);

                manager.Cleanup();
                Thread.Sleep(250);
            }
        }
    }
}