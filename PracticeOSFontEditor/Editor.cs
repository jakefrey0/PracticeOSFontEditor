﻿/*
 * Created by SharpDevelop.
 * User: Elite
 * Date: 2/16/2021
 * Time: 10:53 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace PracticeOSFontEditor {
	
	public partial class Editor : Form {
		
		private List<Byte> dataToWrite;
		
		private static Char[] characters={'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
										  'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
										  '0','1','2','3','4','5','6','7','8','9','!','@','#','$','%','^','&','*','(',')','-','_','=','+','{','}',
										  ';',':','\'','"',',','<','.','>','/','?','\\','|','`','~'};
		
		/// <summary>
		/// characters.Length as a constant
		/// </summary>
		private const Byte characterCount=92;
		
		private Byte userCharacterIndicator;
		
		public Editor (FontType ft) {
			
			const UInt16 x8=
				// 1= first byte to signify whether it is 8x,16x,32x,64x
				1+
				(
					
					(
						// 8 - signify the amount of rows
						8
						*
						// 8 - the bytes per row
						1
					)
					
					*
					//The amount of characters
					//from characters.Length
					Editor.characterCount
					
				),
			
				x16=1+((16*2)*Editor.characterCount),
				x32=1+((32*4)*Editor.characterCount),
				x64=1+((64*8)*Editor.characterCount)
				
			;
				
			switch (ft) {
					
				case FontType.x8:
					this.dataToWrite=new List<Byte>(x8);
					this.dataToWrite.Add(0x00);
					break;
					
				case FontType.x16:
					this.dataToWrite=new List<Byte>(x16);
					this.dataToWrite.Add(0x03);
					break;
					
				case FontType.x32:
					this.dataToWrite=new List<Byte>(x32);
					this.dataToWrite.Add(0x0F);
					break;
					
				case FontType.x64:
					this.dataToWrite=new List<Byte>(x64);
					this.dataToWrite.Add(0x3F);
					break;
					
				default:
					// I don't see how this could ever happen
					MessageBox.Show("An unexpected error has occured.");
					Environment.Exit(1);
					break;
					
			}
			
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			this.InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			
			this.createDrawGrid();
			
		}
		
		private void EditorFormClosed (Object sender, FormClosedEventArgs e) { Environment.Exit(0); }
		
		private void createDrawGrid () {
			
			Int32 dimension=(Int32)(Math.Sqrt((((this.dataToWrite.Capacity-1)/Editor.characterCount)*8))),fixedDimension=dimension-1,difference=(this.drawGridPanel.Height/(dimension));;
			UInt16 ctr=0,x=0,y=0;
			
			while (ctr!=(((dataToWrite.Capacity/Editor.characterCount)*8))) {
				
				Panel p=new Panel(){Width=difference,Height=difference,Left=(x*(difference)),Top=(y*(difference)),BackColor=Color.White,Name=ctr.ToString()+"_drawGridSubPanel"};
				
				p.MouseDown+= delegate (Object sender,MouseEventArgs args) {
					
					if (args.Button==MouseButtons.Left)
						p.BackColor=Color.Black;
					else if (args.Button==MouseButtons.Right)
						p.BackColor=Color.White;
					
				};
				p.MouseMove+= delegate {
					
					if (!(Control.MouseButtons==MouseButtons.Left)&&!(Control.MouseButtons==MouseButtons.Right))
						return;
					
					foreach (Control c in this.drawGridPanel.Controls) {
						
						Point p0=this.drawGridPanel.PointToClient(Editor.MousePosition);
						
						if (new Rectangle(p0.X,p0.Y,1,1).IntersectsWith(c.Bounds))
							c.BackColor=(Control.MouseButtons==MouseButtons.Left)?Color.Black:Color.White;
						
					}
					
				};
				
				this.drawGridPanel.Controls.Add(p);
				if (x==(fixedDimension)) {
					++y;
					x=0;
				}
				else ++x;
				++ctr;
				
			}
			
		}
		
		private void updateLabels () {
			
			this.charactersToGoLabel.Text=(this.userCharacterIndicator.ToString()+'/'+Editor.characterCount.ToString());
			this.drawIndicatorLabel.Text="Draw the character: \""+Editor.characters[this.userCharacterIndicator]+'"';
			
		}
		
		private void EditorLoad (Object sender,EventArgs e) { this.updateLabels(); }
		
		private void DoneButtonClick (Object sender, EventArgs e) {
			
			this.userCharacterIndicator=92;
			if (this.userCharacterIndicator==92) {
				
				//TODO :: savefiledialog
				SaveFileDialog sfd=new SaveFileDialog();
				sfd.Filter="PracticeOS Font Files (*.pfont)|*.pfont";
				sfd.RestoreDirectory=true;
				if (sfd.ShowDialog()==DialogResult.OK) {
					
					this.userCharacterIndicator=0;
					goto done;
					
				}
				
			}
			
			Byte tempByte=0x00,ctr=7;
			
			foreach (Control c in this.drawGridPanel.Controls) {
				
				tempByte |= (Byte)(((c.BackColor==Color.Black)?(Byte)1:(Byte)0) << ctr );
				
				if (ctr==0) {
					
					this.dataToWrite.Add(tempByte);
					
					tempByte=0x00;
					ctr=7;
					
					continue;
					
				}
				
				--ctr;
				
			}
			
			++this.userCharacterIndicator;
			done:
			this.updateLabels();
			this.clearPanels();
			
		}
		
		private void clearPanels () {
			
			//TODO::
			
		}
		
	}
	
}