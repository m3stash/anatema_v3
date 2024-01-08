using System;
using UnityEngine;

namespace RoomUI {
    public static class Utilities {
        public static T? GetEnumValueFromDropdown<T>(string value) where T : struct, Enum {
            T enumValue;
            if (Enum.TryParse(value, out enumValue)) {
                return enumValue;
            }
            Debug.Log("Unknown Enum GetEnumValueFromDropdown: " + value);
            return null;
        }
    }
}

