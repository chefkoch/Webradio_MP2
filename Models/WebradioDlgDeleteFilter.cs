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
  internal class WebradioDlgDeleteFilter : IWorkflowModel 
  {
    public const string MODEL_ID_STR = "59AB04C6-6B8D-41E5-A041-7AFC8DEDEB89";
    public const string NAME = "name";
    public const string ID = "id";

    public static List<MyFilter> FilterList = new List<MyFilter>();

    public ItemsList FilterItems = new ItemsList();

    public WebradioDlgDeleteFilter()
    {
    }

    public void Init()
    {
      FilterList = WebradioFilter.FilterList;
      ImportFilter();
    }

    public void ImportFilter()
    {
      FilterItems.Clear();
      int id = 0;
      foreach (MyFilter mf in FilterList)
      {
        ListItem item = new ListItem();
        item.AdditionalProperties[NAME] = mf.Titel;
        item.AdditionalProperties[ID] = mf.ID;
        item.SetLabel("Name", mf.Titel);
        FilterItems.Add(item);
        id += 1;
      }
      FilterItems.FireChange();
    }

    public void Select(ListItem item)
    {
      if (item.Selected == true)
      {
        item.Selected = false;
      }
      else
      {
        item.Selected = true;
      }
      item.FireChange();
    }

    public void Delete()
    {
      foreach (ListItem item in FilterItems)
      {
        if (item.Selected == true)
        {
          foreach (MyFilter mf in FilterList)
          { 
            if(mf.ID == (string)item.AdditionalProperties[ID])
            {
              FilterList.Remove(mf);
              break;
            }
          }
        }
      }
      WebradioFilter.SaveImage = "Unsaved.png";
      WebradioFilter.FilterTitel = "";
      ImportFilter();
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
