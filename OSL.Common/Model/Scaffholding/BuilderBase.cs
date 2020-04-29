/* Copyright 2020 Nicolas Mayeur

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    https://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using System;

namespace GeoSports.Common.Model.Scaffholding
{
    public abstract class BuilderBase<T>
    {
        public static implicit operator T(BuilderBase<T> builder)
        {
            return builder.Build();
        }

        private bool _built;

        public T Build()
        {
            if (_built)
            {
                throw new InvalidOperationException("Instance already built");
            }

            _built = true;

            return GetInstance();
        }

        protected abstract T GetInstance();

    }
}
