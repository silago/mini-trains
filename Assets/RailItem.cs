// Copyright (C) 2023 White Sharx (https://whitesharx.com) - All Rights Reserved.
// Unauthorized copying of this file, via any medium is strictly prohibited.
// Proprietary and confidential.

using System;
using System.Collections.Generic;
using UnityEngine;

public class RailItem : BuildItem {
  [SerializeField]
  List<RailTrackElement> railTrackElements = new List<RailTrackElement>();

  protected override void Awake() {
    base.Awake();

    railTrackElements.ForEach(x => x.enabled = false);
  }

  public override void Initialize(ItemSettings settings) {
    base.Initialize(settings);

    railTrackElements.ForEach(x => x.enabled = true);
  }
}
