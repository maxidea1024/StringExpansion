# StringExpansion
간단한  C#용 문자열 펼치기 관련한 간단한 조각 코드입니다.

`makefile`이나 `rpm spec` 파일을 사용하다보면, `${name}`, `%(name)` 과 같이 환경변수나 약속한 값들로 치환하는 것을 볼수 있습니다. 이러한 동작을 수행하는 간단한 코드 조각입니다.

- makefile에서 사용하는 예

```makefile
#------
# Name of program or plugin
#------
TARGET_NAME = myproj

ifeq ($(TARGET_TYPE),exe)
  TARGET=$(TARGET_NAME)$(EXE)
else
  TARGET=$(TARGET_NAME)$(DLL)
endif

#------
# Location of sources and object files
#------
SRC=$(wildcard *.cpp)
OBJS=$(addsuffix .o, $(basename $(SRC)))
OUT=.
```

---

`IVarProvider`를 잘 활용하면 다양한 기능을 손쉽게 확장할 수 있습니다. 예를들면, 환경 변수 또는 시스템 경로등을 제공할 수 있으므로, `.config` 파일등에 사용될 수 있습니다.

- 환경 변수를 값으로 제공
```csharp
using System;

namespace StringExpansion.VarProviders
{
    public class EnvironmentVarProvider : IVarProvider
    {
        public static readonly EnvironmentVarProvider DefaultInstance = new EnvironmentVarProvider();
        
        public string GetVar(string name)
        {
            return Environment.GetEnvironmentVariable(name);
        }
    }
}
```

- 시스템 경로를 제공
```csharp
using System;
using System.IO;

namespace StringExpansion.VarProviders
{
    public class PathVarProvider : IVarProvider
    {
        public static readonly PathVarProvider DefaultInstance = new PathVarProvider();
        
        public string GetVar(string name)
        {
            string path = null;
            
            if (name == "home")
            {
                path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            }
            else if (name == "module")
            {
                path = AppContext.BaseDirectory;
            }

            var envVar = Environment.GetEnvironmentVariable(name);
            if (envVar != null)
                path = envVar;

            if (path != null)
                return MakeNonPathTerm(path);

            return null;
        }

        private string MakeNonPathTerm(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            if (path.EndsWith(Path.DirectorySeparatorChar))
                return path.Substring(0, path.Length - 1);

            return path;
        }
    }
}
```

---

저는 현재 boilerplate 코드 생성 도구에 사용하고 있습니다. 제가 사용하고 있는 코드의 한 조각입니다.

- 타입스크림트용 코드를 생성하는 곳에서 사용되고 있는 코드 샘플

```csharp
ts.Verbatim(@"

    // Indexing by '%prop_name'
    public get recordsBy%pascal_name(): Map<%field_type, %record_type> { return this._recordsBy%pascal_name }
    private _recordsBy%pascal_name: Map<%field_type, %record_type> = new Map<%field_type, %record_type>()

    /** Gets the value associated with the specified key. throw Error if not found. */
    public getBy%pascal_name(key: %field_type): %record_type {
        const found = this._recordsBy%pascal_name.get(key)
        if (!found)
            throw new Error(`There is no record in table ""%table_name"" that corresponds to field ""%prop_name"" value ${key}`)

        return found
    }

    /** Gets the value associated with the specified key. */
    public tryGetBy%pascal_name(key: %field_type): %record_type | undefined {
        return this._recordsBy%pascal_name.get(key)
    }

    /** Determines whether the table contains the specified key. */
    public contains%pascal_name(key: %field_type): boolean {
        return !!this._recordsBy%pascal_name.has(key)
    }"
);
```
