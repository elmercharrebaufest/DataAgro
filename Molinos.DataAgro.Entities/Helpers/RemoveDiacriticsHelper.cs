using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;

namespace WebDataAgro.Helpers
{
    public static class RemoveDiacriticsHelper
    {
        public static string RemoveDiacritics(this String s)
        {
            return string.Concat(Regex.Replace(s, @"(?i)[\p{L}-[ña-z]]+", m => m.Value.Normalize(NormalizationForm.FormD))
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark));
        }
    }
}