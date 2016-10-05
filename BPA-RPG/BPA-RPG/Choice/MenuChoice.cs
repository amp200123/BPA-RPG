﻿using BPA_RPG.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPA_RPG.Choice
{
    /// <summary>
    /// Represents a choice in a menu
    /// </summary>
    public class MenuChoice
    {
        public List<string> synopsis { get; private set; }
        public List<ChoiceOption> options { get; private set; }
        private MenuChoiceScreen screen;

        public MenuChoice(MenuChoiceScreen screen, List<string> synopsis, List<ChoiceOption> options)
        {
            this.screen = screen;
            this.synopsis = synopsis;
            this.options = options;
        }

        /// <summary>
        /// Creates a MenuChoice object from text.
        /// </summary>
        /// <param name="lines">Lines of text that represent a choice</param>
        /// <returns></returns>
        public static MenuChoice ChoiceFromText(MenuChoiceScreen screen, List<string> lines)
        {
            int lineNum = 0;
            bool endOfLine = false;
            List<string> synopsis = new List<string>();
            List<ChoiceOption> options = new List<ChoiceOption>();

            //Loop through lines that make up the Choice synopsis
            while (!endOfLine && !lines[lineNum].ToLower().StartsWith(">"))
            {
                synopsis.Add(lines[lineNum]);
                lineNum++;

                if (lineNum >= lines.Count)
                    endOfLine = true;
            }

            //Loop through the options for this choice
            while (!endOfLine)
            {
                List<string> optionLines = new List<string>();
                optionLines.Add(lines[lineNum]);
                lineNum++;

                bool inNewChoice = false;

                while (!endOfLine && (!lines[lineNum].StartsWith(">") || inNewChoice))
                {
                    if (lines[lineNum].StartsWith("choice"))
                        inNewChoice = true;
                    else if (lines[lineNum].StartsWith("}"))
                        inNewChoice = false;

                    optionLines.Add(lines[lineNum]);
                    lineNum++;

                    if (lineNum >= lines.Count)
                        endOfLine = true;
                }
                
                options.Add(ChoiceOption.OptionFromText(screen, optionLines));

                if (lineNum >= lines.Count)
                    endOfLine = true;
            }

            return new MenuChoice(screen, synopsis, options);
        }
    }
}