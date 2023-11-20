        public static void GS_GUILD_BOOKMARK_CREATE_ACK(GS_GUILD_BOOKMARK_CREATE_ACK pkt, IPACKET_ERROR err)
        {
            if (NetErrorHelper.ShowErrorMsg(err)) return;

            int preCount = GuildManager.Instance.MyGuild.GuildBookmarkContainer.Count;
            GuildManager.Instance.MyGuild.GuildBookmarkContainer.Upsert((idx) => pkt.GuildBookmarks(idx), pkt.GuildBookmarksLength);
            int postCount = GuildManager.Instance.MyGuild.GuildBookmarkContainer.Count;

            if (preCount < postCount)
                ToastMessage.Show(Util.Text("COMMON_BOOKMARK_ADD_BOOKMARK_NOTICE"));
        }

        static void GS_GUILD_BOOKMARK_DELETE_ACK(GS_GUILD_BOOKMARK_DELETE_ACK pkt, IPACKET_ERROR err)
        {
            if (NetErrorHelper.ShowErrorMsg(err)) return;

            GuildManager.Instance.MyGuild.GuildBookmarkContainer.Remove((idx) => pkt.BookmarkID);
        }

        public static void GS_GUILD_BOOKMARK_MODIFY_ACK(GS_GUILD_BOOKMARK_MODIFY_ACK pkt, IPACKET_ERROR err)
        {
            if (NetErrorHelper.ShowErrorMsg(err)) return;

            GuildManager.Instance.MyGuild.GuildBookmarkContainer.Upsert((idx) => pkt.GuildBookmarks(idx), pkt.GuildBookmarksLength);

        }

        public static void GS_GUILD_BOOKMARK_UPDATE_NFY(GS_GUILD_BOOKMARK_UPDATE_NFY pkt, IPACKET_ERROR err)
        {
            if (NetErrorHelper.ShowErrorMsg(err)) return;

            GuildManager.Instance.MyGuild.GuildBookmarkContainer.Upsert((idx) => pkt.GuildBookmarks(idx), pkt.GuildBookmarksLength);

        }
