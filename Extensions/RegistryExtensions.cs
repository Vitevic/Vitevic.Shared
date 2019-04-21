using Microsoft.Win32;

namespace Vitevic.Shared.Extensions
{
    public static class RegistryExtensions
    {
        public static RegistryKey Registry32(this RegistryHive registryHive, string subkey)
        {
            return RegistryKey.OpenBaseKey(registryHive, RegistryView.Registry32).OpenSubKey(subkey);
        }

        public static RegistryKey Registry64(this RegistryHive registryHive, string subkey)
        {
            return RegistryKey.OpenBaseKey(registryHive, RegistryView.Registry64).OpenSubKey(subkey);
        }
    }
}
