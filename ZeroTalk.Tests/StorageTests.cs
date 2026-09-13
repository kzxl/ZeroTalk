using System;
using System.IO;
using ZeroTalk.Server.Data;

namespace ZeroTalk.Tests
{
    public static class StorageTests
    {
        public static void RunAll()
        {
            TestUserStoreAuthentication();
            TestUserStoreDuplicateRegistration();
            TestMessageStoreSaveAndRetrieve();
        }

        private static void TestUserStoreAuthentication()
        {
            string tempDb = Path.Combine(Path.GetTempPath(), $"users_{Guid.NewGuid():N}.json");
            try
            {
                var store = new UserStore(tempDb);

                // Default seed users
                Assert.NotNull(store.GetByUsername("alice"));
                Assert.NotNull(store.GetByUsername("bob"));
                Assert.NotNull(store.GetByUsername("charlie"));

                // Valid pass
                string passHash = UserStore.HashPassword("123");
                var user = store.Authenticate("alice", passHash);
                Assert.NotNull(user);
                Assert.Equal("alice", user.Username);

                // Invalid pass
                var failedUser = store.Authenticate("alice", UserStore.HashPassword("wrongpassword"));
                Assert.True(failedUser == null);
            }
            finally
            {
                if (File.Exists(tempDb)) File.Delete(tempDb);
            }
        }

        private static void TestUserStoreDuplicateRegistration()
        {
            string tempDb = Path.Combine(Path.GetTempPath(), $"users_{Guid.NewGuid():N}.json");
            try
            {
                var store = new UserStore(tempDb);
                bool added = store.Register("newUser", UserStore.HashPassword("pass"), "New User");
                Assert.True(added);

                // Duplicate username
                bool duplicateAdded = store.Register("newUser", UserStore.HashPassword("pass2"), "Another User");
                Assert.False(duplicateAdded, "Duplicate usernames should be rejected");
            }
            finally
            {
                if (File.Exists(tempDb)) File.Delete(tempDb);
            }
        }

        private static void TestMessageStoreSaveAndRetrieve()
        {
            string tempDir = Path.Combine(Path.GetTempPath(), $"chat_history_{Guid.NewGuid():N}");
            try
            {
                var msgStore = new MessageStore(tempDir);
                msgStore.SaveMessage("u1", "u2", "user1", "Hello u2!", false);
                msgStore.SaveMessage("u2", "u1", "user2", "Hey u1, received loud and clear.", false);

                var history = msgStore.GetHistory("u1", "u2");
                Assert.NotNull(history);
                Assert.Equal(2, history.Count);
                Assert.Equal("Hello u2!", history[0].Content);
                Assert.Equal("Hey u1, received loud and clear.", history[1].Content);
            }
            finally
            {
                if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
            }
        }
    }
}
