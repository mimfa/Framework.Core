using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization;
using System.IO;
using System.Reflection;

namespace MiMFa.General
{
    public class Transmission
    {
        public static T DeepCopy<T>(T other)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, other);
                ms.Position = 0;
                return (T)formatter.Deserialize(ms);
            }
        }
        public static void Copy<T,F>(T source, F destination)
        {
            if (source == null || destination == null) return;
            var sfi = source.GetType().GetFields();
            var dfi = destination.GetType().GetFields();
            foreach (var sourceField in sfi)
                foreach (var destField in dfi)
                    if (destField.Name == sourceField.Name &&
                      destField.FieldType.IsAssignableFrom(sourceField.FieldType))
                       try { destField.SetValue(destination, sourceField.GetValue(source)); } catch { }
            var spi = source.GetType().GetProperties();
            var dpi = destination.GetType().GetProperties();
            foreach (var sourceProperty in spi)
                foreach (var destProperty in dpi)
                    if (destProperty.Name == sourceProperty.Name &&
                      destProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
                        try { destProperty.SetValue(destination, sourceProperty.GetValue(source)); } catch { }
        }
    }
}
