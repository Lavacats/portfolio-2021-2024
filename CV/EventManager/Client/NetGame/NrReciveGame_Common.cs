 protected void GS_DAILY_EVENT_DATA_GET_ACK(GS_DAILY_EVENT_DATA_GET_ACK pkt)
        {
            EVENT.EventManager.Instance.AddDailyEventData(pkt);

            EVENT.EventManager.Instance.dailyEventPoint = pkt.DailyPoint;
            EVENT.EventManager.Instance.dailyEventWeeklyPoint = pkt.WeeklyPoint;

            // 수령된 보상 리스트 갱신
            EVENT.EventManager.Instance.dailyEventRewardSteps.Clear();
            List<Int32> dailyRewardSteps = new List<Int32>();
            for (int i = 0; i < pkt.RewardStepLength; ++i)
            {
                EVENT.EventManager.Instance.dailyEventRewardSteps.Add(pkt.GetRewardStep(i));
            }

            TotalEventListDlg EventListDlg = UIFormManager.Instance.FindUIForm<TotalEventListDlg>();
            if (pkt.ShowUI == 1 && null != EventListDlg && false == EventListDlg.Visible)
            {
                EventListDlg.Show();
                UIFormManager.Instance.OpenUIForm<EventDailyDlg>();
            }

            if (EventListDlg != null)
            {
                if (false == UIFormManager.Instance.IsOpenUIForm<EventDailyDlg>())
                    UIFormManager.Instance.OpenUIForm<EventDailyDlg>();
                else
                    EventListDlg.RefreshDailyEvnetUI();
            }
        }

        protected void GS_RANKING_DAILY_SET_ACK(GS_RANKING_DAILY_SET_ACK pkt)
        {
            if (EVENT.EventManager.Instance != null)
                EVENT.EventManager.Instance.PointUpdate(pkt.DailyPoint, pkt.WeeklyPoint);

            GameEventHandler.Instance.AddDispatchedEvent(new DailyEventDataUpdated());

            var dailyEvenDlg = UIFormManager.Instance.FindUIForm<EventDailyDlg>();
            if (dailyEvenDlg != null)
                dailyEvenDlg.DailyEventSliderUpdate();

        }
 protected void GS_DAILY_EVENT_RANKINFO_GET_ACK(GS_DAILY_EVENT_RANKINFO_GET_ACK pkt)
        {
            if (pkt.Result == 0)
            {
                RANKING.RankManager.Instance.Set_Daily_Event_RankInfo_Get_Ack(pkt);
            }
        }
        protected void GS_SEASON_EVENT_INFO_ACK(GS_SEASON_EVENT_INFO_ACK pkt)
        {
            TotalEventListDlg EventListDlg = UIFormManager.Instance.FindUIForm<TotalEventListDlg>();
            if (null != EventListDlg && true == EventListDlg.Visible)
            {
                if (false == UIFormManager.Instance.IsOpenUIForm<EventSeasonDlg>()
                   && UIFormManager.Instance.IsOpenUIForm<TotalEventListDlg>())
                {
                    if (pkt.ShowUI == 1)
                    {
                        ChloeStoryEventManager.Instance.Set_Season_Event_MissionInfo_Get_Ack(pkt);
                        UIFormManager.Instance.OpenUIForm<EventSeasonDlg>();
                        var seasonEventUi = UIFormManager.Instance.GetUIForm<EventSeasonDlg>();

                        seasonEventUi.SetSeasonEventUI(pkt.CurSeason, pkt.CurPeriod);
                        seasonEventUi.RefreshSeasonEvent();
                    }
                }
            }

        }
        protected void GS_SEASON_EVENT_INFO_NFY(GS_SEASON_EVENT_INFO_NFY pkt)
        {
            if (pkt.MissionLength > 0)
            {
                ChloeStoryEventManager.Instance.Set_Season_Event_MissionInfo_Get_NFY(pkt);
            }
        }
        protected void GS_USE_EVENT_LOGIN_NFY(GS_USE_EVENT_LOGIN_NFY pkt)
        {
            if (pkt.EventListLength != 0)
            {
                UseEventManager.Instance.Set_Use_Event_Get_LOGIN_NFY(pkt);
            }
        }
        protected void GS_USE_EVENT_NFY(GS_USE_EVENT_NFY pkt)
        {
            if (pkt.EventGroupKind != 0)
            {
                UseEventManager.Instance.Set_Use_Event_Get_NFY(pkt);
            }
        }
        protected void GS_USE_EVENT_INFO_ACK(GS_USE_EVENT_INFO_ACK pkt)
        {
            if (pkt.Result != 0)
            {
                NDebug.LogWarning("GS_USE_EVENT_INFO_ACK fail. ");
                return;
            }

            if (0 != pkt.EventListLength)
            {
                UseEventManager.Instance.Set_Use_Event_Get_ACK(pkt);
            }

            if (UIFormManager.Instance.IsOpenUIForm<EventGemUseDlg>())
            {
                var seasonEventUi = UIFormManager.Instance.GetUIForm<EventGemUseDlg>();
                seasonEventUi.RefreshUI();
            }
            else
            {
                UIFormManager.Instance.OpenUIForm<EventGemUseDlg>();
            }
        }
        protected void GS_USE_EVENT_REWARD_ACK(GS_USE_EVENT_REWARD_ACK pkt)
        {
            if (pkt.Result != 0)
            {
                NDebug.LogWarning("GS_USE_EVENT_REWARD_ACK fail. ");
                return;
            }

            if (pkt.EventGroupKind>0)
            {
                int groupKind   = pkt.EventGroupKind;
                int eventKind   = pkt.EventKind;
                int poingGrade  = pkt.PointGrade;

                // 매니저에 추가
                UseEventManager.Instance.InsertRewardInfo(eventKind, groupKind, poingGrade);

                // UI 갱신
                if (UIFormManager.Instance.IsOpenUIForm<EventGemUseDlg>())
                {
                    var useEventUi = UIFormManager.Instance.GetUIForm<EventGemUseDlg>();
                    useEventUi.RefreshSlider(eventKind);
                }
            }
        }
        protected void GS_SEASON_EVENT_REWARD_ACK(GS_SEASON_EVENT_REWARD_ACK pkt)
        {
            if (0 != pkt.Result)
            {
                ErrorNetworkHandler.OnError(pkt.Result);
                return;
            }

            // 보상
            for (int i = 0; i < pkt.RewardLength; ++i)
            {
                var evnetRewardInfo = pkt.GetReward(i);
                if (evnetRewardInfo != null)
                {
                    if (ChloeStoryEventManager.Instance.dicRewardEvent.ContainsKey(evnetRewardInfo.SeasonEventKind))
                    {
                        ChloeStoryEventManager.Instance.dicRewardEvent[evnetRewardInfo.SeasonEventKind] = evnetRewardInfo.Result;
                    }
                    else
                    {
                        ChloeStoryEventManager.Instance.dicRewardEvent.Add(evnetRewardInfo.SeasonEventKind, evnetRewardInfo.Result);
                    }

                    if (evnetRewardInfo.GetRewardItem(0).ItemKind != 0)
                    {
                        if (UIFormManager.Instance.IsOpenUIForm<EventSeasonDlg>())
                        {
                            var seasonEventUi = UIFormManager.Instance.GetUIForm<EventSeasonDlg>();
                            seasonEventUi.SetRewardEffect(evnetRewardInfo.SeasonEventKind);
                            seasonEventUi.RefreshSeasonEvent();
                        }

                        foreach (var missionInfo in TableChloeEventInfo.Instance.m_chloeEventDataInfoList)
                        {
                            if (missionInfo.Value.SeasonEventKind == evnetRewardInfo.SeasonEventKind)
                            {
                                if (missionInfo.Value.missionCardNum == 0)
                                {
                                    UIFormManager.Instance.OpenUIForm<EventSeasonClearDlg>();

                                }
                            }
                        }
                        Inventory.Instance.AddItemNumAndRefreshNewItem(evnetRewardInfo.GetRewardItem(0).ItemKind, evnetRewardInfo.GetRewardItem(0).ItemNum);
                    }
                }
            }
        }


        protected void GS_DAILY_EVENT_DATA_GET_ACK(GS_DAILY_EVENT_DATA_GET_ACK pkt)
        {
            EVENT.EventManager.Instance.AddDailyEventData(pkt);

            EVENT.EventManager.Instance.dailyEventPoint = pkt.DailyPoint;
            EVENT.EventManager.Instance.dailyEventWeeklyPoint = pkt.WeeklyPoint;

            // 수령된 보상 리스트 갱신
            EVENT.EventManager.Instance.dailyEventRewardSteps.Clear();
            List<Int32> dailyRewardSteps = new List<Int32>();
            for (int i = 0; i < pkt.RewardStepLength; ++i)
            {
                EVENT.EventManager.Instance.dailyEventRewardSteps.Add(pkt.GetRewardStep(i));
            }

            TotalEventListDlg EventListDlg = UIFormManager.Instance.FindUIForm<TotalEventListDlg>();
            if (pkt.ShowUI == 1 && null != EventListDlg && false == EventListDlg.Visible)
            {
                EventListDlg.Show();
                UIFormManager.Instance.OpenUIForm<EventDailyDlg>();
            }

            if (EventListDlg != null)
            {
                if (false == UIFormManager.Instance.IsOpenUIForm<EventDailyDlg>())
                    UIFormManager.Instance.OpenUIForm<EventDailyDlg>();
                else
                    EventListDlg.RefreshDailyEvnetUI();
            }
        }
