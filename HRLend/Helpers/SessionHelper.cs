using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Helpers.Session
{
    class SessionDataWithExpiration<T>
    {
        public T Value { get; }
        public DateTime ExpirationTime { get; }

        public SessionDataWithExpiration(T value, DateTime expirationTime)
        {
            Value = value;
            ExpirationTime = expirationTime;
        }
    }

    public static class SessionHelper
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize<T>(value));
        }

        public static T? Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonSerializer.Deserialize<T>(value);
        }

        public static void SetWithExpiration<T>(this ISession session, string key, T value, TimeSpan expirationTime)
        {
            var serializedValue = JsonSerializer.Serialize<SessionDataWithExpiration<T>>(new SessionDataWithExpiration<T>(value, DateTime.UtcNow.Add(expirationTime)));
            session.SetString(key, serializedValue);
        }

        public static T? GetWithExpiration<T>(this ISession session, string key)
        {
            var serializedValue = session.GetString(key);
            if (serializedValue == null)
            {
                return default(T);
            }

            var sessionData = JsonSerializer.Deserialize<SessionDataWithExpiration<T>>(serializedValue);
            if (sessionData.ExpirationTime < DateTime.UtcNow)
            {
                session.Remove(key);
                return default(T);
            }

            return sessionData.Value;
        }
    }
}
