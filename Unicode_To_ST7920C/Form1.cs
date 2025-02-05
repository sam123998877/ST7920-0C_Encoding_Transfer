﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Unicode_To_ST7920F
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void tbxInput_TextChanged(object sender, EventArgs e)
        {
            if (tbxInput.Text.Length <= 0)
            {
                tbxOutput.Text = "";
                return;
            }

            StringBuilder errorList = new StringBuilder();
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < tbxInput.Text.Length; i++)
            {
                int ch = tbxInput.Text[i];
                int uv = -1;

                if(ch >= 128)
                {
                    int lb = 0, ub = ST7920F_UnicodeMapping.table.GetLength(0);
                    //int mid = 0;

                    while (lb <= ub)
                    {
                        /*
                        mid = (lb + ub) / 2;
                        if (ch == ST7920F_UnicodeMapping.table[mid, 0])
                        {
                            uv = ST7920F_UnicodeMapping.table[mid, 1];
                            break;
                        }
                        else if (ch > ST7920F_UnicodeMapping.table[mid, 0])
                        {
                            lb = mid + 1;
                        }
                        else
                        {
                            ub = mid;
                        }*/

                        if (ch == ST7920F_UnicodeMapping.table[lb, 0])
                        {
                            uv = ST7920F_UnicodeMapping.table[lb, 1];
                            break;
                        }
                        
                        if (lb == ub){
                            //No match
                            break;
                        }
                        lb++;
                    }
                }
                else
                {
                    uv = ch;
                }

                if (uv >= 0 && uv < ushort.MaxValue)
                {
                    if (rdbStringStyle.Checked)
                    {
                        if (result.Length == 0 && cbParentheses.Checked) result.Append("{");
                        if (uv >= 0x20 && uv < 127 && uv != '\\')
                        {
                            result.Append((char)uv);
                        }
                        else
                        {
                            result.Append("\\x");
                            if (uv >= 256)
                            {
                                result.Append(((uv >> 8) & 0xFF).ToString("X"));
                                result.Append("\\x");
                            }
                            result.Append((uv & 0xFF).ToString("X"));
                        }
                    }
                    else if (rdbArrayStyle.Checked)
                    {
                        if (result.Length > 0) result.Append(", ");
                        if (result.Length == 0 && cbParentheses.Checked) result.Append("{");
                        result.Append("0x");
                        if (uv >= 256)
                        {
                            result.Append(((uv >> 8) & 0xFF).ToString("X"));
                            result.Append(", 0x");
                        }
                        result.Append((uv & 0xFF).ToString("X"));
                    }
                    else if (rdb8051Style1.Checked) {
                        if (result.Length > 0) result.Append(",");
                        if (result.Length == 0 && cbParentheses.Checked) result.Append("{");
                        if (cbPatch.Checked) result.Append("0");
                        if (uv >= 256)
                        {
                            result.Append(((uv >> 8) & 0xFF).ToString("X"));
                            result.Append("H,");
                        }
                        if (cbPatch.Checked) result.Append("0");
                        result.Append((uv & 0xFF).ToString("X"));
                        result.Append("H");
                    }
                    else if (rdb8051Style2.Checked)
                    {
                        if (result.Length > 0) result.Append(",");
                        if (result.Length == 0 && cbParentheses.Checked) result.Append("{");
                        if (cbPatch.Checked) result.Append("0");
                        if (uv >= 256)
                        {
                            result.Append(((uv >> 8) & 0xFF).ToString("X"));
                        }
                        result.Append((uv & 0xFF).ToString("X"));
                        result.Append("H");
                    }
                    else
                    {
                        result.Clear();
                        result.Append("你到底是字串還是陣列搞得我好亂啊");
                        break;
                    }
                }
                else
                {
                    if (errorList.Length == 0) errorList.Append("轉換錯誤：");
                    errorList.Append(string.Format("{0}(0x{1}) ", i, ch.ToString("X")));
                }
            }
            if (cbParentheses.Checked) result.Append("}");
            tbxOutput.Text = result.ToString();
            statusMessage.Text = errorList.ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            statusMessage.Text = "字庫大小：" + ST7920F_UnicodeMapping.table.GetLength(0).ToString();
        }

        private void tbxOutput_MouseClick(object sender, MouseEventArgs e)
        {
            tbxOutput.SelectAll();
        }

        private void rdb8051Style1_CheckedChanged(object sender, EventArgs e)
        {
            tbxInput_TextChanged(sender, e);
        }

        private void cbParentheses_CheckedChanged(object sender, EventArgs e)
        {
            tbxInput_TextChanged(sender, e);
        }

        private void rdb8051Style2_CheckedChanged(object sender, EventArgs e)
        {
            tbxInput_TextChanged(sender, e);
        }

        private void cbPatch_CheckedChanged(object sender, EventArgs e)
        {
            tbxInput_TextChanged(sender, e);
        }
    }
}
