// Copyright 2011-2012 Chris Patterson
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace OdoyuleRules.Designer
{
    using System;
    using System.Linq.Expressions;


    public interface JoinDesigner<TLeft, TRight>
        where TLeft : class
        where TRight : class
    {
        JoinDesigner<TLeft, TRight> When(Expression<Func<TLeft, bool>> expression);
        JoinDesigner<TLeft, TRight> When(Expression<Func<TRight, bool>> expression);
        JoinDesigner<TLeft, TRight> When(Expression<Func<TLeft, TRight, bool>> expression);

        JoinDesigner<TLeft, TRight> Then(Action<ThenDesigner<TLeft>> leftAction);
        JoinDesigner<TLeft, TRight> Then(Action<ThenDesigner<TRight>> rightAction);
        JoinDesigner<TLeft, TRight> Then(Action<ThenDesigner<TLeft, TRight>> configureCallback);
    }
}