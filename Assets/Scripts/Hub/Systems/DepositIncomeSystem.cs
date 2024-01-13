using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using YAPCG.Hub.Components;

namespace YAPCG.Hub.Systems
{
    public partial struct DepositIncomeSystem : ISystem
    {
        private BufferLookup<Deposit.Sizes> _depositSizesLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _depositSizesLookup = SystemAPI.GetBufferLookup<Deposit.Sizes>(true);

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {  
            _depositSizesLookup.Update(ref state);
            var incomeJob = new IncomeJob() { DepositSizesLookup = _depositSizesLookup };
            incomeJob.Run();
        }

        
        [BurstCompile]
        partial struct IncomeJob : IJobEntity
        {
            [ReadOnly] public BufferLookup<Deposit.Sizes> DepositSizesLookup;

            void Execute( in Entity entity, ref Deposit.Reserves reserves)
            {
                DynamicBuffer<Deposit.Sizes> sizesBuffer = DepositSizesLookup[entity];
                for (int i = 0; i < sizesBuffer.Length; i++)
                    reserves.Value += (i + 1) * sizesBuffer[i].Size;
            }
        }
    }
    

}