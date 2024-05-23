TransactionStore
Приватный API, предназначенный для хранения транзакций лидов (что-то вроде бухгалтерской книги). Принимает запросы только от CRM из соображений безопасности. Все созданные транзакции находят своё отображение в репортинг-сервисе

функционал:
запись транзакций
получение транзакции по Id
получение транзакций по AccountId (особое внимание уделить трансферным транзакциям)
получение баланса аккаунта
микросервис надеется на то, что он обладает информацией об актуальных курсах валют. если курсы валют не получены, или же если курсы валют уже не актуальны, трансферные транзации он принимать не может. сам микросервис инфу о курсах валют не запрашивает

бизнес-логика:
микросервис может получить инфу о текущей базовой валюте вместе с курсами валют (базовая валюта может поменяться, да)
курсы валют организованы в виде перечня валютных пар, где ключом выступает название валютной пары (например, USDRUB), а значением - отношение одной единице первой в паре валюты к одной единице второй валюты (например, 1USD / 1 RUB = 77.31)
если на текущий момент базовой является валюта USD, то микросервис настроен на работу с валютными парами вида: USDRUB, USDEUR, USDJPY и т.д.
если базовой валютой станет EUR, то микросервис будет искать инфу о валютных парах EURRUB, EURUSD и т.д.
транзакция на депозит (с типом Deposit) представляется в БД одной строкой с положительным Amount’ом
транзакция на снятие (с типом Withdraw) представляется в БД одной строкой м отрицательным Amount’ом
транзакция на трансфер (с типом Transfer) представляется в БД двумя строками. в первой строке значится аккаунт, с которого переводят сумму (в этом случае значение суммы отрицательное), во второй строке значится аккаунт, на который происходит зачисление денежных средств (в этом случае значение суммы положительное). примечание: в обеих строках значение суммы указывается в валюте аккаунта (т.е. если идёт трансфер с USD на RUB, то в первой строке может быть Amount -1, а во второй будет Amount 77)
несмотря на то, что транзакция в БД хранится в виде двух строк, с точки зрения бизнес-логики, такая транзакция является одним объектом. это означает, что если контроллер принимает запрос на создание трансферной транзакции, в объекте запроса будут указаны акк, с которого происходит трансфер, и акк, на который происходит трансфер. при этом сумма транзакции означает денежные средства, которые будут переведены с указанного акка (т.е. вторую сумму нужно вычислить, используя имеющиеся курсы валют)
это же нужно учесть при получении транзакций по AccountId (в этом случае это будет список из транзакций всех трёх типов, и трансферные транзакции  должны быть добавлены в список если с указанного акка происходил перевод и если на указанный аккаунт происходил перевод. при этом, в объекте трансферной транзакции должна быть полная информация: сколько денег пришло с одного аккаунта и сколько денег пришло на другой аккаунт)
отдельно нужно обезопасить систему от двойного снятия денежных средств: при одновременном получении двух запросов на трансфер или снятие нужно не допустить выполнения второй транзакции, если после проведения первой транзакции баланс акка исчерпался. если баланса хватает на обе транзакции, то провести обе транзакции

дополнительные требования:
для каждого лида в системе должно быть создано кол-во транзакций, равное leadAccountsCount*3, из них 20% - депозит, 10% - снятие, 70% - трансферы
при генерации данных баланс акков может “уйти в минус”, это мы будем отлавливать в репортинг-сервисе
покрытие тестами каждого проекта в солюшне должно быть не менее 80%
логи микросервиса должны писаться в текстовый файлик. путь до файлика нужно читать из энв-переменной