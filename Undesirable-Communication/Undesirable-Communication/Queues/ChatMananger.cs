using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Undesirable_Communication.Model.Connection;
using Undesirable_Communication.Model.User;

namespace Undesirable_Communication.Queues
{
    public class ChatMananger
    {
        private static ChatMananger manager;
        private Queue<RegisteredUser> PendingUsers {get;set;}

        private List<ChatConnection> CurrentConnections { get; set; }

        private readonly object PendingUsersLock;
        private readonly object CurrentConnectionsLock;

        private ChatMananger()
        {
            PendingUsersLock = new object();
            lock (PendingUsersLock)
            {
                CurrentConnectionsLock = new object();
                lock (CurrentConnectionsLock)
                {
                    PendingUsers = new Queue<RegisteredUser>();
                    CurrentConnections = new List<ChatConnection>();
                }
            }
            
        }

        public static ChatMananger GetInstance()
        {
            if(manager == null)
            {
                manager = new ChatMananger();
            }

            return manager;
        }

        public void EnqueuePendingUser(RegisteredUser theUser) {
            lock (this.PendingUsersLock)
            {
                this.PendingUsers.Enqueue(theUser);
            }
        }

        public RegisteredUser DequeuePendingUser()
        {
            lock (this.PendingUsersLock)
            {
                return this.PendingUsers.Dequeue();
            }
        }

        public Guid? MakeConnectionIfPossible()
        {
            lock (this.PendingUsersLock)
            {
                if(this.PendingUsers.Count <= 1)
                {
                    return null;
                }

                var user1 = this.PendingUsers.Dequeue();
                var user2 = this.PendingUsers.Dequeue();

                var newConnectionGuid = new Guid();

                var newConnection = new ChatConnection { User1 = user1, User2 = user2, Id = newConnectionGuid, StartTime = DateTime.Now };

                lock (this.CurrentConnectionsLock)
                {
                    this.CurrentConnections.Add(newConnection);

                    return newConnectionGuid;
                }
            }
        }

        public T QueryPendingUsers<T>(Func<Queue<RegisteredUser>, T> funcToRun)
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
    }
}