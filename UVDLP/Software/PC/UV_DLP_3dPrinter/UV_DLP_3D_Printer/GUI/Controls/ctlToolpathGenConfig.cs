﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
namespace UV_DLP_3D_Printer.GUI.Controls
{
    public partial class ctlToolpathGenConfig : UserControl
    {
        public ctlToolpathGenConfig()
        {
            try
            {
                InitializeComponent();
                PopulateProfiles();
                lbGCodeSection.SelectedIndex = 0;
            }catch(Exception)
            {
            
            }
        }

       private SliceBuildConfig m_config = null;
        // this populates the profile in use and the combo 
       private void PopulateProfiles() 
       {
           try
           {
               cmbSliceProfiles.Items.Clear();
               lstSliceProfiles.Items.Clear();
               foreach (string prof in UVDLPApp.Instance().SliceProfiles())
               {
                   cmbSliceProfiles.Items.Add(prof);
                   lstSliceProfiles.Items.Add(prof);
               }
               //get the current profile name
               string curprof = UVDLPApp.Instance().GetCurrentSliceProfileName();
               cmbSliceProfiles.SelectedItem = curprof;
               lstSliceProfiles.SelectedItem = curprof;
           }
           catch (Exception ex) 
           {
               DebugLogger.Instance().LogError(ex.Message);
           }
       }
       private string CurPrefGcodePath() 
       {
           try
           {
               string shortname = lstSliceProfiles.SelectedItem.ToString();
               string fname = UVDLPApp.Instance().m_PathProfiles;
               fname += UVDLPApp.m_pathsep + shortname + UVDLPApp.m_pathsep;
               return fname;
           }
           catch (Exception ex) 
           {
               DebugLogger.Instance().LogError(ex.Message);
               return "";
           }
       }
        private SliceBuildConfig LoadProfile(string shortname) 
        {
            SliceBuildConfig profile = new SliceBuildConfig();
            try
            {
                string fname = UVDLPApp.Instance().m_PathProfiles;
                fname += UVDLPApp.m_pathsep + shortname + ".slicing";
                if (!profile.Load(fname))
                {
                    DebugLogger.Instance().LogError("Could not load " + fname);
                    return null;
                }
                else 
                {
                    return profile;
                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex.Message);
            }
            return null;
        }
        private void SetValues() 
        {
            lblProfName.Text = lstSliceProfiles.SelectedItem.ToString();
            txtZThick.Text = "" + String.Format("{0:0.000}",m_config.ZThick);
            chkExport.Checked = m_config.export;
            groupBox2.Enabled = chkExport.Checked;
           // chkgengcode.Checked = m_config.exportgcode;
           // chkExportSlices.Checked = m_config.exportimages;
            //chkexportsvg.Checked = m_config.exportsvg;
            if (m_config.m_exportopt.ToUpper().Contains("ZIP"))
            {
                rbzip.Checked = true;
            }
            else 
            {
                rbsub.Checked = true;
            }
            txtLayerTime.Text = "" + m_config.layertime_ms;
            txtFirstLayerTime.Text = m_config.firstlayertime_ms.ToString();
            txtBlankTime.Text = m_config.blanktime_ms.ToString();
            txtXOffset.Text = m_config.XOffset.ToString();
            txtYOffset.Text = m_config.YOffset.ToString();
            txtLiftDistance.Text = m_config.liftdistance.ToString();
            txtnumbottom.Text = m_config.numfirstlayers.ToString();
            txtSlideTilt.Text = m_config.slidetiltval.ToString();
            chkantialiasing.Checked = m_config.antialiasing;
            chkmainliftgcode.Checked = m_config.usemainliftgcode;
            grpLift.Enabled = !chkmainliftgcode.Checked;
            txtliftfeed.Text = m_config.liftfeedrate.ToString();
            txtretractfeed.Text = m_config.liftretractrate.ToString();
            chkReflectX.Checked = m_config.m_flipX;
            chkReflectY.Checked = m_config.m_flipY;
            txtNotes.Text = m_config.m_notes;
            txtResinPriceL.Text = m_config.m_resinprice.ToString();

           // txtRaiseTime.Text = m_config.raise_time_ms.ToString();

            foreach(String name in Enum.GetNames(typeof(SliceBuildConfig.eBuildDirection)))
            {
                cmbBuildDirection.Items.Add(name);
            }
            cmbBuildDirection.SelectedItem = m_config.direction.ToString();
        }

        private bool GetValues() 
        {
            try
            {
                
                m_config.ZThick = Single.Parse(txtZThick.Text);
                if (rbzip.Checked == true)
                {
                    m_config.m_exportopt = "ZIP";
                }
                else 
                {
                   m_config.m_exportopt = "SUBDIR";
                }
                m_config.layertime_ms = int.Parse(txtLayerTime.Text);
                m_config.firstlayertime_ms = int.Parse(txtFirstLayerTime.Text);
                m_config.blanktime_ms = int.Parse(txtBlankTime.Text);
                m_config.XOffset = int.Parse(txtXOffset.Text);
                m_config.YOffset = int.Parse(txtYOffset.Text);
                m_config.liftdistance = double.Parse(txtLiftDistance.Text);
                m_config.numfirstlayers = int.Parse(txtnumbottom.Text);
                m_config.slidetiltval = double.Parse(txtSlideTilt.Text);
                m_config.antialiasing = chkantialiasing.Checked;
                m_config.usemainliftgcode = chkmainliftgcode.Checked;
                m_config.liftfeedrate = double.Parse(txtliftfeed.Text);
                m_config.liftretractrate = double.Parse(txtretractfeed.Text);
                //  m_config.raise_time_ms = int.Parse(txtRaiseTime.Text);
                grpLift.Enabled = !chkmainliftgcode.Checked;
                m_config.m_flipX = chkReflectX.Checked;
                m_config.m_flipY = chkReflectY.Checked;
                m_config.m_notes = txtNotes.Text;
                m_config.m_resinprice = double.Parse(txtResinPriceL.Text);
                m_config.direction = (SliceBuildConfig.eBuildDirection)Enum.Parse(typeof(SliceBuildConfig.eBuildDirection), cmbBuildDirection.SelectedItem.ToString());
                m_config.export = chkExport.Checked;
                return true;
            }
            catch (Exception ex) 
            {
                MessageBox.Show("Please check input parameters\r\n" + ex.Message,"Input Error");
                return false;
            }
        }


        private void chkmainliftgcode_CheckedChanged(object sender, EventArgs e)
        {
            grpLift.Enabled = !chkmainliftgcode.Checked;
        }
        /*
        private void chkgengcode_CheckedChanged(object sender, EventArgs e)
        {

        }
        */
        private void cmdAutoCalc_Click(object sender, EventArgs e)
        {
            try
            {
                if (GetValues())
                {
                    double zlift = m_config.liftdistance; // in mm
                    double zliftrate = m_config.liftfeedrate; // in mm/m
                    double zliftretract = m_config.liftretractrate; // in mm/m
                    zliftrate /= 60.0d;     // to convert to mm/s
                    zliftretract /= 60.0d;  // to convert to mm/s

                    double tval = 0;
                    double settlingtime = 2500.0d; // 500 ms
                    tval = (zlift / zliftrate);
                    tval += (zlift / zliftretract);
                    tval *= 1000.0d; // convert to ms
                    tval += settlingtime;
                    String stime = ((int)tval).ToString();
                    txtBlankTime.Text = stime;

                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex.Message);
            }
        }

        private void cmdApply_Click(object sender, EventArgs e)
        {
            if(m_config == null) return;
            if (lstSliceProfiles.SelectedIndex == -1) return;
            try
            {
                if (GetValues())
                {
                    string shortname = lstSliceProfiles.SelectedItem.ToString();
                    string fname = UVDLPApp.Instance().m_PathProfiles;
                    fname += UVDLPApp.m_pathsep + shortname + ".slicing";
                    m_config.Save(fname);
                    // make sure main build params are updated if needed
                    if (cmbSliceProfiles.SelectedItem.ToString() == shortname)
                    {
                        UVDLPApp.Instance().LoadBuildSliceProfile(fname);
                    }
                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex.Message);
            }
        }

        private void cmbSliceProfiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            // get the item
            if (cmbSliceProfiles.SelectedIndex == -1)
            {
                //blank items
                return;
            }
            else 
            {
                //set this profile to be the active one for the program                
                string shortname = cmbSliceProfiles.SelectedItem.ToString();
                string fname = UVDLPApp.Instance().m_PathProfiles;
                fname += UVDLPApp.m_pathsep + shortname + ".slicing";
                UVDLPApp.Instance().LoadBuildSliceProfile(fname);                 
            }
        }

        private void lstSliceProfiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            // get the item
            if (lstSliceProfiles.SelectedIndex == -1)
            {
                //blank items
                return;
            }
            else
            {
                string shortname = lstSliceProfiles.SelectedItem.ToString();
                m_config = LoadProfile(shortname);
                if (m_config != null)
                {
                    SetValues();
                }
                else 
                {
                    //blank items
                }
                 
            }
        }

        private void cmdNew_Click(object sender, EventArgs e)
        {
            // prompt for a new name
            frmProfileName frm = new frmProfileName();
            if (frm.ShowDialog() == DialogResult.OK) 
            {
                //create a new profile
                SliceBuildConfig bf = new SliceBuildConfig();
                //save it
                string shortname = frm.ProfileName;
                string fname = UVDLPApp.Instance().m_PathProfiles;
                fname += UVDLPApp.m_pathsep + shortname + ".slicing";
                if (!bf.Save(fname)) 
                {
                    MessageBox.Show("Error saving new profile " + fname);
                }
                //re-display the new list
                PopulateProfiles();
            }
        }

        private void cmdDelete_Click(object sender, EventArgs e)
        {
            try
            {
                string shortname = lstSliceProfiles.SelectedItem.ToString();
                if (shortname.ToLower().Contains("default"))
                {
                    MessageBox.Show("Cannot delete default profile");
                }
                else
                {

                    string fname = UVDLPApp.Instance().m_PathProfiles;
                    fname += UVDLPApp.m_pathsep + shortname;
                    string pname = fname;
                    fname += ".slicing";
                    File.Delete(fname); // delete the file
                    Directory.Delete(pname, true);
                    PopulateProfiles();
                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex.Message);
            }
        }

        /// <summary>
        /// this index changes when the user selects an item from the list of GCode file segements
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbGCodeSection_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtGCode.Text = GCodeSection2GCode();
        }

        private string GCodeSection2GCode()
        {
            if (lbGCodeSection.SelectedIndex == -1) return "";
            switch (lbGCodeSection.SelectedIndex)
            {
                case 0: return m_config.HeaderCode;
                case 1: return m_config.PreSliceCode;
                case 2: return m_config.PreLiftCode;
                case 3: return m_config.PostLiftCode;
                case 4: return m_config.FooterCode;
                case 5: return m_config.MainLiftCode;
            }
            return "";
        }

        private string GCodeSection2FName() 
        {
            if (lbGCodeSection.SelectedIndex == -1) return "";
            switch (lbGCodeSection.SelectedIndex) 
            {
                case 0: return "start.gcode";
                case 1: return "preslice.gcode";
                case 2: return "prelift.gcode";
                case 3: return "postlift.gcode";
                case 4: return "end.gcode";
                case 5: return "mainlift.gcode";
            }
            return "";
        }

        private void cmdSaveGCode_Click(object sender, EventArgs e)
        {
            try
            {
                // save the gcode to the right section
                string gcode = txtGCode.Text;
                if (lbGCodeSection.SelectedIndex == -1) return;
                switch (lbGCodeSection.SelectedIndex)
                {
                    case 0: m_config.HeaderCode = gcode; break;
                    case 1: m_config.PreSliceCode = gcode; break;
                    case 2: m_config.PreLiftCode = gcode; break;
                    case 3: m_config.PostLiftCode = gcode; break;
                    case 4: m_config.FooterCode = gcode; break;
                    case 5: m_config.MainLiftCode = gcode; break;
                }
                m_config.SaveFile(CurPrefGcodePath() + GCodeSection2FName(), gcode);
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex.Message);
            }
        }

        private void cmdReloadGCode_Click(object sender, EventArgs e)
        {
            txtGCode.Text = GCodeSection2GCode();
        }

        private void chkExport_CheckedChanged(object sender, EventArgs e)
        {
            groupBox2.Enabled = chkExport.Checked;
        }

        private void chkmainliftgcode_CheckedChanged_1(object sender, EventArgs e)
        {
            grpLift.Enabled = !chkmainliftgcode.Checked;
        }
    }
}