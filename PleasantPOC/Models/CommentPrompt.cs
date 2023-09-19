namespace PleasantPOC.Models
{
    public class CommentPrompts
    {
        public bool AskForCommentOnViewPassword { get; set; } = false;
        public bool AskForCommentOnViewOffline { get; set; } = false;
        public bool AskForCommentOnModifyEntries { get; set; } = false;
        public bool AskForCommentOnMoveEntries { get; set; } = false;
        public bool AskForCommentOnMoveFolders { get; set; } = false;
        public bool AskForCommentOnModifyFolders { get; set; } = false;
    }
}