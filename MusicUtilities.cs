using JuanMartin.Kernel;
using JuanMartin.Models.Music;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JuanMartin.MusicStudio
{
    public static class MusicUtilities
    {

        public static string ParseChanelHeader(string chanel)
        {
            StringBuilder header = new StringBuilder();
            bool inHeader = false;

            foreach (var c in chanel)
            {
                if (c == Measure.MeasureHeaderEnd)
                    inHeader = false;
                else if (c == Measure.MeasureHeaderStart)
                    inHeader = true;

                if (inHeader && c != Measure.MeasureHeaderStart && c != Measure.MeasureHeaderEnd)
                    header.Append(c);
            }
            return header.ToString();
        }

        public static string[] FixStaffDelimiters(string staffs)
        {
            int i = 0;
            var measures = staffs.Split(new char[] { Measure.MeasureDelimiter }, options: StringSplitOptions.RemoveEmptyEntries);

            foreach (string measure in measures)
            {
                if (!string.IsNullOrEmpty(measure))
                {
                    string aux = measure.Trim();

                    // deal with end measures
                    if (aux.Length > 0 && aux.Last() != Measure.MeasureDelimiter)
                    {
                        aux = $"{aux} {Measure.MeasureDelimiter}";
                        measures[i] = aux;
                    }
                    // deal with openning of chanels
                    bool isInStaff = false;
                    int position = 0;
                    foreach (char c in aux)
                    {
                        string tail = "";
                        if (position < aux.Length)
                            tail = aux.Substring(position + 1);

                        if (!isInStaff && c == Measure.MeasureDelimiter && tail.Length > 0)
                            isInStaff = true;
                        else if (c == Measure.MeasureDelimiter && isInStaff)
                            isInStaff = false;
                        position++;
                    }
                    if (!isInStaff)
                    {
                        measures[i] = $"{Measure.MeasureDelimiter} {aux}";
                    }
                }
                i++;
            }

            return measures;
        }

    }
}
