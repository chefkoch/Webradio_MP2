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
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
  internal class WebradioDlgShowFavorites : IWorkflowModel
  {
    #region Consts

    public const string MODEL_ID_STR = "9723DCC8-969D-470E-B156-F4E6E639DD18";
    public const string NAME = "name";

    public string SelectedStream = "";
    public bool _changed = false;

    #endregion

    public static List<MyFavorit> FavoritList = new List<MyFavorit>();

    public ItemsList FavoritItems = new ItemsList();
    

    public WebradioDlgShowFavorites()
    {
    }

    public void Init()
    {
      FavoritList = MyFavorits.Read().FavoritList;      
      ImportFavorits(Convert.ToString(WebradioHome.SelectedStream.ID));
      SelectedStream = WebradioHome.SelectedStream.Titel;
    }


    public void ImportFavorits(string _ID)
    {
      FavoritItems.Clear();
      foreach (MyFavorit f in FavoritList)
      {
        ListItem item = new ListItem();
        item.AdditionalProperties[NAME] = f.Titel;
        item.SetLabel("Name", f.Titel);
        if (f.IDs.Contains(_ID))
        {
          item.Selected = true;
        }
        else
        {
          item.Selected = false;
        }
        FavoritItems.Add(item);
      }
    }

    /// <summary>
    /// Import selected Filter
    /// </summary>
    public void SelectFavorite(ListItem item)
    {
      List<MyStream> list = new List<MyStream>();
      foreach (MyFavorit f in FavoritList)
      {
        if (f.Titel == (string)item.AdditionalProperties[NAME]) 
        {
          IEnumerable<MyStream> query = from r in WebradioHome.StreamList where _contains(f.IDs, Convert.ToString(r.ID)) select r;
          foreach (MyStream ms in query)
          {
            if (!list.Contains(ms)) { list.Add(ms); }
          }
          break;
        }
      }
      WebradioHome.FillItemList(list);
    }

    private static bool _contains(List<string> L, string S)
    {
      if (L.Count == 0) { return true; }

      string[] split = S.Split(new Char[] { ',' });
      foreach (string s in split)
      {
        if (L.Contains(s))
        {
          return true;
        }
      }
      return false;
    }

    public void SetFavorite(ListItem item)
    {
      string s = (string)item.AdditionalProperties[NAME];
      string id = Convert.ToString(WebradioHome.SelectedStream.ID);

      foreach (MyFavorit f in FavoritList)
      {
        if (f.Titel == s)
        {
          if (item.Selected == true)
          {
            item.Selected = false;
            f.IDs.Remove(id);
          }
          else
          {
            item.Selected = true;
            f.IDs.Add(id);
          }
          _changed = true;
        }
      }
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
      Init();
    }

    public void ExitModelContext(NavigationContext oldContext, NavigationContext newContext)
    {
      if (_changed == true) 
      {
        MyFavorits.Write(new MyFavorits(FavoritList));
      }
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
