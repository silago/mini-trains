// Copyright (C) 2023 White Sharx (https://whitesharx.com) - All Rights Reserved.
// Unauthorized copying of this file, via any medium is strictly prohibited.
// Proprietary and confidential.

using System.Collections.Generic;
using UnityEngine;

public class CrossroadItem : BuildItem {
  [SerializeField]
  private List<RailItem> railItems;
  
  [SerializeField]
  private RailSwitcher switcher;

  public override void Initialize(int size) {
    base.Initialize(size);

    foreach (var railItem in railItems) {
      railItem.Initialize(size);
    }
    
    switcher.Initialize();
  }
}
