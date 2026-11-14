using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityGuide.Services
{
    public static class EncryptionAndDecryption
    {
        const char CharacterToFill = '@';

        static public string EnCrypt(string str, string Key)
        {
            int count;
            int ctr;
            //Key = "";
            int size = str.Length;
            string R = "";
            for (count = 0; count < size; ++count)
            {
                ctr = (str[count] >> 2) & 0x3f; R += Key[ctr];
                ctr = (str[count] << 4) & 0x3f;
                if (++count < size)
                    ctr |= (str[count] >> 4) & 0x0f;
                R += Key[ctr];
                if (count < size)
                {
                    ctr = (str[count] << 2) & 0x3f;
                    if (++count < size)
                        ctr |= (str[count] >> 6) & 0x03;
                    R += Key[ctr];
                }
                else
                {
                    ++count;
                    R += CharacterToFill;
                }
                if (count < size)
                {
                    ctr = str[count] & 0x3f;
                    R += Key[ctr];
                }
                else
                {
                    R += CharacterToFill;
                }
            }
            return (R);
        }
        static public string DeCrypt(string str, string Key)
        {
            string R = "";
            int count;
            char ctr;
            //Key = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
            char ctrNext;
            int size = str.Length;
            for (count = 0; count < size; ++count)
            {
                try
                {
                    ctr = (char)Key.IndexOf(str[count]);
                    ++count;
                    ctrNext = (char)Key.IndexOf(str[count]);
                    ctr = ((char)((ctr << 2) | ((ctrNext >> 4) & 0x3)));
                    R += ctr;
                    if (++count < size)
                    {
                        ctr = str[count];
                        if (CharacterToFill == ctr)
                            break;
                        ctr = (char)Key.IndexOf(ctr);
                        ctrNext = (char)(((ctrNext << 4) & 0xf0) | ((ctr >> 2) & 0xf));
                        R += ctrNext;
                    }
                    if (++count < size)
                    {
                        ctrNext = str[count];
                        if (CharacterToFill == ctrNext)
                            break;
                        ctrNext = (char)Key.IndexOf(ctrNext);
                        ctr = (char)(((ctr << 6) & 0xc0) | ctrNext);
                        R += ctr;
                    }
                }
                catch
                {
                }
            }
            return (R);
        }
    }
}