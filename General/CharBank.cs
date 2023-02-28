using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiMFa.General
{
    public class CharBank
    {
        //English
        public char[] EnglishCharacters = {'a','b','c','d',
                                    'e','f','g','h',
                                    'i','j','k','l',
                                    'm','n','o','p',
                                    'q','r','s','t',
                                    'u','v','w','x',
                                    'y','z',
                                    'A','B','C','D',
                                    'E','F','G','H',
                                    'I','J','K','L',
                                    'M','N','O','P',
                                    'Q','R','S','T',
                                    'U','V','W','X',
                                    'Y','Z'};
        public char[] EnglishUpperCharacters = {
                                    'A','B','C','D',
                                    'E','F','G','H',
                                    'I','J','K','L',
                                    'M','N','O','P',
                                    'Q','R','S','T',
                                    'U','V','W','X',
                                    'Y','Z'};
        public char[] EnglishLowerCharacters = {
                                    'a','b','c','d',
                                    'e','f','g','h',
                                    'i','j','k','l',
                                    'm','n','o','p',
                                    'q','r','s','t',
                                    'u','v','w','x',
                                    'y','z',};
        //Persian
        public char[] FarsiCharacters = {'ا','ب','پ','ت',
                                   'ث','ج','چ','ح',
                                   'خ','د','ذ','ر',
                                   'ز','ژ','س','ش',
                                   'ص','ض','ط','ظ',
                                   'ع','غ','ف','ق',
                                   'ک','گ','ل','م',
                                   'ن','و','ه','ی',
                                   'ء','آ','ـ'};
        //Number
        public char[] NumberCharacters = {'1','2','3','4',
                                    '5','6','7','8',
                                    '9','0'};
        //Sign
        public char[] SignCharacters = {' ','~','!','@','#',
                                 '$','%','^','&',
                                 '*','(',')','_',
                                 '-','+','=','/',
                                 '\\','[',']','{',
                                 '}',';',':',',',
                                 '.','?','<','>','`'};
        //Symbol
        public char[] SymbolCharacter = {'☺','☻','♥','♦','♣',
                                     '♠','•','◘','○',
                                     '◙','♂','♀','♪',
                                     '♫','☼','►','◄',
                                     '↕','‼','¶','§',
                                     '▬','↨','↑','↓',
                                     '→','←','∟','↔',
                                     '▲','▼'};
        //
    }
}
