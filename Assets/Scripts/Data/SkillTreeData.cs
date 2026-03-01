using UnityEngine;

namespace UWG.Data
{
    [CreateAssetMenu(fileName = "NewSkillTree", menuName = "UWG/Skill Tree")]
    public class SkillTreeData : ScriptableObject
    {
        public string treeName;
        public SkillBranch branch;
        [TextArea(1, 3)]
        public string description;
        public SkillNodeData[] nodes;
    }
}
