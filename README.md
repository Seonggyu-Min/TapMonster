# TapMonster

### Unity Version: 2022.3.62f2

## 컨벤션

### 커밋 탬플릿
| 타입 (type) | 설명 |
|-------------|------|
| `feat`      | 새로운 기능 추가, 기존의 기능을 요구 사항에 맞추어 수정 |
| `fix`       | 기능에 대한 버그 수정 |
| `set`       | 단순 파일 추가 |
| `build`     | 빌드 관련 수정 |
| `chore`     | 그 외 기타 수정 |
| `ci`        | CI 관련 설정 수정 |
| `docs`      | 문서(주석) 수정 |
| `style`     | 코드 스타일, 포맷팅에 대한 수정 |
| `refactor`  | 기능의 변화가 아닌 코드 리팩터링 |
| `test`      | 테스트 코드 추가/수정 |
| `release`   | 버전 릴리즈 |

### 브랜치 규칙
1. `main` 브랜치 - 항상 실행 가능하고 안정적인 상태를 유지하는 기준 브랜치
2. `feature/*` 브랜치 - 새로운 기능 추가나 개선 작업을 기능 단위로 진행하기 위한 작업 브랜치

### 코드 컨벤션
프로젝트는 아래의 코드 컨벤션을 따라 작성됩니다. 


| 요소                    | 규칙                | 예시                                  |
| --------------------- | ----------------- | ----------------------------------- |
| **클래스 / 인터페이스**       | `PascalCase`      | `PlayerController`, `IGameService`  |
| **메서드**               | `PascalCase`      | `StartGame()`, `GetData()`          |
| **변수 / 필드 (private)** | `camelCase`       | `playerName`, `currentHealth`       |
| **상수 / readonly 필드**  | `대문자` + `_`      | `MAX_HEALTH`, `DEFAULT_SPEED`         |
| **이벤트**               | `PascalCase` + 동사 | `OnDamageTaken`, `PlayerDied`       |
| **로컬 변수**             | `camelCase`       | `index`, `tempScore`                |
| **enum 타입**           | `PascalCase`      | `PlayerState` / `Idle`, `Running` 등 |
| **제네릭 타입 매개변수**       | `T` 접두어 사용        | `TEntity`, `TResult`                |

`private`는 `_`로 시작하여 카멜 규칙을 적용합니다.

`상수 / readonly` 필드는 링크의 규칙을 따르지 않고 위의 표의 규칙을 따릅니다.

> 링크: https://learn.microsoft.com/ko-kr/dotnet/csharp/fundamentals/coding-style/identifier-names

<br><br>

| 항목                 | 스타일                            | 예시                            |
| ------------------ | ------------------------------ | ----------------------------- |
| **중괄호 `{}`**       | 항상 새 줄에                        | `if ()\n{\n}`                 |
| **들여쓰기**           | 공백 4칸                          | VS 기본 설정                      |
| **공백 규칙**          | 연산자 양 옆 공백                     | `x = y + z;`                  |
| **줄바꿈**            | 논리 단위로 구분                      | 함수 간 한 줄 띄움                   |
| **파일 하나에 하나의 클래스** | 권장                             | `ClassA.cs`, `ClassB.cs` 따로   |
| **`this.` 사용**     | 선택적 (모호할 때만)                   | `this.name = name;`           |
| **접근제한자 순서**       | `public → protected → private` | 클래스/메서드/필드 모두 해당              |
| **네임스페이스**         | PascalCase로                    | `namespace MyApp.Controllers` |

> 링크: https://learn.microsoft.com/ko-kr/dotnet/csharp/fundamentals/coding-style/coding-conventions

<br>
