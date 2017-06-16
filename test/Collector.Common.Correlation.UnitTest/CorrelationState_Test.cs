namespace Collector.Common.Correlation.UnitTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Xunit;

    public class CorrelationState_Test : IDisposable
    {
        [Fact]
        public void When_correlation_is_access_different_threads_It_returns_different_ids()
        {
            var thread1 = Task<Guid?>.Factory.StartNew(() =>
                                                       {
                                                           CorrelationState.InitializeCorrelation();

                                                           var correlationid = CorrelationState.GetCurrentCorrelationId();

                                                           CorrelationState.ClearCorrelation();

                                                           return correlationid;

                                                       });
            var thread2 = Task<Guid?>.Factory.StartNew(() =>
                                                       {
                                                           CorrelationState.InitializeCorrelation();

                                                           var correlationid = CorrelationState.GetCurrentCorrelationId();

                                                           CorrelationState.ClearCorrelation();

                                                           return correlationid;
                                                       });
            Task.WaitAll(thread1, thread2);

            Assert.NotEqual(thread1.Result, thread2.Result);
        }

        [Fact]
        public void When_correlation_is_set_with_a_predefined_correlation_id_Then_it_should_always_return_it()
        {
            var correlationId1 = Guid.NewGuid();
            var correlationId2 = Guid.NewGuid();

            var thread1 = Task<Guid?>.Factory.StartNew(() =>
                                                       {
                                                           CorrelationState.InitializeCorrelation(correlationId1);

                                                           var correlationid = CorrelationState.GetCurrentCorrelationId();

                                                           CorrelationState.ClearCorrelation();

                                                           return correlationid;
                                                       });

            var thread2 = Task<Guid?>.Factory.StartNew(() =>
                                                       {
                                                           CorrelationState.InitializeCorrelation(correlationId2);

                                                           var correlationid = CorrelationState.GetCurrentCorrelationId();

                                                           CorrelationState.ClearCorrelation();

                                                           return correlationid;
                                                       });

            Task.WaitAll(thread1, thread2);

            Assert.Equal(correlationId1, thread1.Result);
            Assert.Equal(correlationId2, thread2.Result);
        }

        [Fact]
        public void When_correlation_is_uninitialized_Then_it_returns_null_when_asked_for_current_correlation_id()
        {
            var thread = Task<Guid?>.Factory.StartNew(() =>
                                                      {
                                                          CorrelationState.ClearCorrelation();
                                                          return CorrelationState.GetCurrentCorrelationId();
                                                      });

            Task.WaitAll(thread);

            Assert.Null(thread.Result);
        }

        [Fact]
        public void When_correlation_is_initialized_and_cleared_Then_it_returns_null_when_asked_for_current_correlation_id()
        {
            var thread = Task<Guid?>.Factory.StartNew(() =>
                                                      {
                                                          CorrelationState.InitializeCorrelation();

                                                          CorrelationState.ClearCorrelation();

                                                          return CorrelationState.GetCurrentCorrelationId();
                                                      });

            Task.WaitAll(thread);

            Assert.Null(thread.Result);
        }

        [Fact]
        public void When_correlation_is_initialized_then_correlation_dictionary_accepts_new_correlation_keyvalues()
        {
            var key = Guid.NewGuid().ToString();
            var expectedValue = Guid.NewGuid().ToString();

            var thread = Task<IEnumerable<KeyValuePair<string, object>>>.Factory.StartNew(() =>
                                                                                          {
                                                                                              CorrelationState.InitializeCorrelation();

                                                                                              CorrelationState.TryAddOrUpdateCorrelationValue(key, expectedValue);

                                                                                              return CorrelationState.GetCorrelationValues();
                                                                                          });

            Task.WaitAll(thread);

            var actualValue = thread.Result.SingleOrDefault(c => c.Key == key);
            Assert.Equal(expectedValue, actualValue.Value);
        }

        [Fact]
        public void When_correlation_is_initialized_and_correlation_value_is_updated_then_the_previous_value_of_the_same_key_is_overwritten()
        {
            var key = Guid.NewGuid().ToString();
            var expectedValue = Guid.NewGuid().ToString();

            var thread = Task<IEnumerable<KeyValuePair<string, object>>>.Factory.StartNew(() =>
                                                                                          {
                                                                                              CorrelationState.InitializeCorrelation();

                                                                                              CorrelationState.TryAddOrUpdateCorrelationValue(key, Guid.NewGuid().ToString());
                                                                                              CorrelationState.TryAddOrUpdateCorrelationValue(key, expectedValue);

                                                                                              return CorrelationState.GetCorrelationValues();
                                                                                          });

            Task.WaitAll(thread);

            var actualValue = thread.Result.SingleOrDefault(c => c.Key == key);
            Assert.Equal(expectedValue, actualValue.Value);
        }

        [Fact]
        public void When_correlation_is_initialized_and_then_cleared_then_it_forgets_the_correlation_keyvalues()
        {
            var thread = Task<IEnumerable<KeyValuePair<string, object>>>.Factory.StartNew(() =>
                                                                                          {
                                                                                              CorrelationState.InitializeCorrelation();

                                                                                              CorrelationState.TryAddOrUpdateCorrelationValue(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

                                                                                              CorrelationState.ClearCorrelation();

                                                                                              return CorrelationState.GetCorrelationValues();
                                                                                          });

            Task.WaitAll(thread);

            Assert.False(thread.Result.Any());
        }

        [Fact]
        public void When_correlation_is_not_initialized_then_it_forgets_the_correlation_keyvalues()
        {
            CorrelationState.TryAddOrUpdateCorrelationValue(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            var values = CorrelationState.GetCorrelationValues();

            Assert.False(values.Any());
        }

        [Fact]
        public async Task When_correlation_id_is_set_outside_an_async_call_then_the_correlation_id_is_available_inside_the_async_call()
        {
            var expectedCorrelationId = Guid.NewGuid();

            CorrelationState.InitializeCorrelation(expectedCorrelationId);

            var correlationId = await Task.Run(() => CorrelationState.GetCurrentCorrelationId());

            Assert.Equal(expectedCorrelationId, correlationId);
        }

        [Fact]
        public async Task When_correlation_values_is_set_outside_an_async_call_then_the_correlation_values_are_available_inside_the_async_call()
        {
            var key = Guid.NewGuid().ToString();
            var value = Guid.NewGuid().ToString();

            CorrelationState.InitializeCorrelation();
            CorrelationState.TryAddOrUpdateCorrelationValue(key, value);

            var correlationValues = (await Task.Run(() => CorrelationState.GetCorrelationValues())).ToList();

            Assert.Equal(1, correlationValues.Count);

            var firstCorrelationValue = correlationValues.First();

            Assert.Equal(key, firstCorrelationValue.Key);
            Assert.Equal(value, firstCorrelationValue.Value);
        }

        [Fact]
        public async Task When_correlation_values_is_set_inside_an_async_call_then_the_correlation_values_are_available_outside_the_async_call()
        {
            var key = Guid.NewGuid().ToString();
            var value = Guid.NewGuid().ToString();

            CorrelationState.InitializeCorrelation();

            await Task.Run(() => CorrelationState.TryAddOrUpdateCorrelationValue(key, value));

            var correlationValues = CorrelationState.GetCorrelationValues().ToList();

            Assert.Equal(1, correlationValues.Count);

            var firstCorrelationValue = correlationValues.First();

            Assert.Equal(key, firstCorrelationValue.Key);
            Assert.Equal(value, firstCorrelationValue.Value);
        }

        public void Dispose()
        {
           CorrelationState.ClearCorrelation();
        }
    }
}
