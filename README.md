# AzureFunction-TimeoutCheck
This repo checks different timeouts at different function configurations on consumption plan.

# Content

## Azure Functions
|Project|Runtime|Description|
|-|-|-|
| [DefaultFunction-RT3](https://github.com/tzuehlke/AzureFunction-TimeoutCheck/tree/main/DefaultFunction-RT3) | ~3 | long running azure function with parameters `runs` (count of iterations) and `value` (time consuming caluclation value) |
| [DefaultFunction-RT4](https://github.com/tzuehlke/AzureFunction-TimeoutCheck/tree/main/DefaultFunction-RT4) | ~4 | same as above |

## Durable Functions
|Project|Runtime|Description|
|-|-|-|
| [DurableFunction-RT3](https://github.com/tzuehlke/AzureFunction-TimeoutCheck/tree/main/DurableFunction-RT3) | ~3 | whole calculation in one called `ActivityTrigger` function |
| [DurableFunction-RT4](https://github.com/tzuehlke/AzureFunction-TimeoutCheck/tree/main/DurableFunction-RT4) | ~4 | calculation is separated shot calls of activity function |