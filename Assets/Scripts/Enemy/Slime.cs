using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;


public class Slime : Enemy
{
    [SerializeField] private GameObject shadow;
    protected override void InitializeSkills()
    {
        AddSkill(new SlimeDash(this));
        AddSkill(new SlimeJump(this));
    }

    public void JumpUp()
    {
        shadow.SetActive(true);
        GetComponent<PolygonCollider2D>().enabled = false;
    }

    public void JumpDown()
    {
        shadow.SetActive(false);
        GetComponent<PolygonCollider2D>().enabled = true;
    }

    private class SlimeDash : EnemySkill
    {
        public SlimeDash(Enemy caster) : base(caster) { }
        public override string skillName => "Slime Dash";
        protected override int previewIndex => 0;
        protected override string animatorTrigger => "Slime_Dash";

        protected override SkillData CalculateSkillData()
        {
            Vector2 direction = (GameManager.Instance.playerInstance.transform.position - caster.transform.position).normalized;
            Vector2 position = direction * 0.75f;
            return new SkillData(position, direction);
        }

        protected override void UpdatePreviewTransform()
        {
            GameObject preview = CurrentPreview;
            if (preview == null) return;

            // 更新位置
            RectTransform previewRectTransform = preview.GetComponent<RectTransform>();
            previewRectTransform.position = Camera.main.WorldToScreenPoint
                (new Vector2(skillData.position.x + caster.transform.position.x, skillData.position.y + caster.transform.position.y));

            float angle = Mathf.Atan2(skillData.direction.y, skillData.direction.x) * Mathf.Rad2Deg;
            previewRectTransform.rotation = Quaternion.Euler(0, 0, angle + 180f);
        }

        public override IEnumerator Execute()
        {
            // 播放動畫
            if (animatorTrigger != null)
            {
                caster.animator.SetTrigger(animatorTrigger);
            }

            // 衝刺動作
            Vector2 dashVelocity = skillData.direction * caster.stats.force;
            caster.rb.linearVelocity = dashVelocity;
            HidePreview();
            yield return new WaitUntil(BattleManager.Instance.AllObjectsStopped);
            caster.animator.SetTrigger("Slime_Idle");
        }
    }

    private class SlimeJump : EnemySkill
    {
        Slime slime => caster as Slime;
        public SlimeJump(Enemy caster) : base(caster) { }
        public override string skillName => "Slime Jump";
        protected override int previewIndex => 1;
        protected override string animatorTrigger => "Slime_Jump";

        protected override SkillData CalculateSkillData()
        {
            Vector2 direction = Vector2.zero;
            Vector2 position = GameManager.Instance.playerInstance.transform.position;
            return new SkillData(position, direction);
        }

        protected override void UpdatePreviewTransform()
        {
            GameObject preview = CurrentPreview;
            if (preview == null) return;

            // 更新位置
            preview.transform.position = new Vector3(skillData.position.x, skillData.position.y, preview.transform.position.z);
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
            yield return new WaitForSeconds(0.5f);
            slime?.JumpUp();

            Vector2 targetPos = skillData.position;
            Vector2 startPos = caster.transform.position;
            float moveTime = 0.5f;
            float elapsed = 0f;
            
            while (elapsed < moveTime)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / moveTime;
                
                float x = Mathf.Lerp(startPos.x, targetPos.x, t);
                float y = Mathf.Lerp(startPos.y, targetPos.y, t);
                
                caster.transform.position = new Vector2(x, y);
                yield return null;
            }
            
            foreach ( IBattleEntity entity in hits )
            {
                if ( entity is Player player )
                {
                    Vector2 dir = player.transform.position - caster.transform.position;
                    player.rb.AddForce(dir * caster.stats.force * 10, ForceMode2D.Impulse);
                    player.TakeDamage(caster.stats.GetAttack());
                    Debug.Log("hits");
                }
            }

            slime.JumpDown();

            yield return new WaitForSeconds(0.5f);
        }
    }
}
