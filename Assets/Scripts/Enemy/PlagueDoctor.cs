using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlagueDoctor : Enemy
{
    protected override void InitializeSkills()
    {
        AddSkill(new PlagueDoctorSlash(this));
        AddSkill(new PlagueDoctorScream(this));
    }

    private class PlagueDoctorSlash : EnemySkill
    {
        public PlagueDoctorSlash(Enemy caster) : base(caster) { }
        public override string skillName => "PlagueDoctor Slash";
        protected override int previewIndex => 0;
        protected override string animatorTrigger => "PlagueDoctor_Slash";

        protected override SkillData CalculateSkillData()
        {
            return new SkillData(Vector2.zero, Vector2.zero);
        }

        protected override void UpdatePreviewTransform()
        {
            return;
        }

        public override IEnumerator Execute()
        {
            // 播放動畫
            if (animatorTrigger != null)
            {
                caster.animator.SetTrigger(animatorTrigger);
            }

            List<IBattleEntity> hits = CurrentPreview.GetComponent<ColliderHits>().GetHits().ToList();
            HidePreview();
            yield return new WaitForSeconds(0.2f);

            foreach ( IBattleEntity entity in hits )
            {
                if ( entity is Player player )
                {
                    Vector2 dir = player.transform.position - caster.transform.position;
                    player.rb.AddForce(dir.normalized * caster.stats.force * 10, ForceMode2D.Impulse);
                    player.TakeDamage(caster.stats.GetAttack());
                    Debug.Log("hits");
                }
            }

            yield return new WaitForSeconds(0.2f);
            yield return new WaitUntil(BattleManager.Instance.AllObjectsStopped);
        }
    }

    private class PlagueDoctorScream : EnemySkill
    {
        public PlagueDoctorScream(Enemy caster) : base(caster) { }
        public override string skillName => "PlagueDoctor Scream";
        protected override int previewIndex => 1;
        protected override string animatorTrigger => "PlagueDoctor_Scream";

        protected override SkillData CalculateSkillData()
        {
            return new SkillData(Vector2.zero, Vector2.zero);
        }

        protected override void UpdatePreviewTransform()
        {
            return;
        }

        public override IEnumerator Execute()
        {
            // 播放動畫
            if (animatorTrigger != null)
            {
                caster.animator.SetTrigger(animatorTrigger);
            }

            HidePreview();
            yield return new WaitForSeconds(1f);
        }
    }
}
