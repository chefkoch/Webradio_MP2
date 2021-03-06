﻿#region Copyright (C) 2007-2013 Team MediaPortal

/*
    Copyright (C) 2007-2013 Team MediaPortal
    http://www.team-mediaportal.com

    This file is part of MediaPortal 2

    MediaPortal 2 is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    MediaPortal 2 is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with MediaPortal 2. If not, see <http://www.gnu.org/licenses/>.
*/

#endregion

using System;
using System.IO;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Globalization;
using System.Linq;
using MediaPortal.Common;
using MediaPortal.Common.General;
using MediaPortal.Common.Settings;
using MediaPortal.UI.Presentation.DataObjects;
using MediaPortal.UI.Presentation.Models;
using MediaPortal.UI.Presentation.Workflow;

namespace Webradio.Models
{
  public class WebradioFavorites : IWorkflowModel

  {
    public const string MODEL_ID_STR = "EC2F9DD4-C694-4C2D-9EFB-092AA1F4BD94";
    
    public const string NAME = "name";
    public const string ID = "id";

    public static List<MyFavorit> FavoritList = new List<MyFavorit>();
    public static ItemsList FavoritItems = new ItemsList();

    public string SelectedID = "";

    private static AbstractProperty _TitelProperty = null;
    public AbstractProperty TitelProperty
    {
      get { return _TitelProperty; }
    }
    public string SelectedTitel
    {
      get { return (string)_TitelProperty.GetValue(); }
      set
      {
        _TitelProperty.SetValue(value);
      }
    }

    private static AbstractProperty _SaveImage = null;
    public AbstractProperty SaveImageProperty
    {
      get { return _SaveImage; }
    }
    public string SaveImage
    {
      get { return (string)_SaveImage.GetValue(); }
      set
      {
        _SaveImage.SetValue(value);
      }
    }

    public WebradioFavorites()
    {
    }

    /// <summary>
    /// Remove a Entry
    /// </summary>
    public void Delete()
    {
      if (SelectedID != "")
      { 
        FavoritList.RemoveRange(Convert.ToInt32(SelectedID), 1);
        ImportFavorits();
        SelectedTitel = "";
        SelectedID = "";
      }
    }

    /// <summary>
    /// Rename a Entry
    /// </summary>
    public void Rename()
    {
      if (SelectedID != "")
      {
        int id = 0;
        foreach (MyFavorit mf in FavoritList)
        {
          if (id == Convert.ToInt32(SelectedID))
          {
            mf.Titel = SelectedTitel;
            break;
          }
          id += 1;
        }
        ImportFavorits();
        SaveImage = "Unsaved.png";
      }
    }

    /// <summary>
    /// Added a Entry
    /// </summary>
    public void Add()
    {
      FavoritList.Add(new MyFavorit("New Favorite", true, new List<string>()));
      ImportFavorits();
      SelectedTitel = "New Favorite";
    }

    /// <summary>
    /// Save all Changes on Site
    /// </summary>
    public void Save()
    {
      MyFavorits.Write(new MyFavorits(FavoritList));
      SaveImage = "Saved.png";
    }

    public void Selected(ListItem item)
    {
      SelectedTitel = (string)item.AdditionalProperties[NAME];
      SelectedID = (string)item.AdditionalProperties[ID];
    }

    private void ImportFavorits()
    {
      FavoritItems.Clear();
      int id = 0;
      foreach (MyFavorit f in FavoritList)
      {
        ListItem item = new ListItem();
        item.AdditionalProperties[NAME] = f.Titel;
        item.AdditionalProperties[ID] = Convert.ToString(id);
        item.SetLabel("Name", f.Titel);
        id += 1;
        FavoritItems.Add(item);
      }
      FavoritItems.FireChange();
    }

    #region IWorkflowModel implementation
    public Guid ModelId
    {
        get { return new Guid(MODEL_ID_STR); }
    }

    public bool CanEnterState(NavigationContext oldContext, NavigationContext newContext)
    {
        return true;
    }

    public void EnterModelContext(NavigationContext oldContext, NavigationContext newContext)
    {
      _SaveImage = new WProperty(typeof(string), string.Empty);
      _TitelProperty = new WProperty(typeof(string), string.Empty);
      FavoritList = MyFavorits.Read().FavoritList;
      ImportFavorits();
      SaveImage = "Saved.png";
    }

    public void ExitModelContext(NavigationContext oldContext, NavigationContext newContext)
    {
    }

    public void ChangeModelContext(NavigationContext oldContext, NavigationContext newContext, bool push)
    {
        // We could initialize some data here when changing the media navigation state
    }

    public void Deactivate(NavigationContext oldContext, NavigationContext newContext)
    {
    }

    public void Reactivate(NavigationContext oldContext, NavigationContext newContext)
    {
    }

    public void UpdateMenuActions(NavigationContext context, IDictionary<Guid, WorkflowAction> actions)
    {
    }

    public ScreenUpdateMode UpdateScreen(NavigationContext context, ref string screen)
    {
        return ScreenUpdateMode.AutoWorkflowManager;
    }
    #endregion

  }
}
