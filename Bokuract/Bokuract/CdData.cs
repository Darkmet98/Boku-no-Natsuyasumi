﻿//
//  CdIndex.cs
//
//  Author:
//       Benito Palacios Sánchez <benito356@gmail.com>
//
//  Copyright (c) 2014 Benito Palacios Sánchez
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace Bokuract
{
    public class CdData : IFormat
	{
		public string FormatName { get { return "Boku1.DIF_DATA"; } }

		public GameFolder Root { get; private set; }

		public void Read(DataStream strIn)
		{
			CdIndex index = (CdIndex)File.Dependencies["cdimg.idx"].Format;

			// Generate file system
			Queue<CdIndexEntry> entries = new Queue<CdIndexEntry>(index.Entries);
			while (entries.Count > 0)
				GiveFormat(entries, this.File);
		}

		private void GiveFormat(Queue<CdIndexEntry> entries, Node folder)
		{
			CdIndexEntry entry = entries.Dequeue();
			if (!entry.IsFolder) {
				folder.AddFile(new GameFile(
					entry.Name,
					new DataStream(this.File.Stream, entry.Offset, entry.Size)
				));
				return;
			}

			// Create the folder
			GameFolder currFolder = new GameFolder(entry.Name, folder);

			// Add files and folders
			for (int i = 0; i < entry.SubEntries - 1; i++)
				GiveFormat(entries, currFolder);
		}
    }
}

